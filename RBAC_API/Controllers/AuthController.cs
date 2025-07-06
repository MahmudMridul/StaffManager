using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RBAC_API.Common;
using RBAC_API.Database;
using RBAC_API.Models;
using RBAC_API.Models.DTOs;
using RBAC_API.Services;
using RBAC_API.Servies;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace RBAC_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RbacContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly SignupValidationService _signUpValidationService;
        private readonly IConfiguration _config;
        private readonly JwtService _jwtService;

        public AuthController(RbacContext context, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config, SignupValidationService signupValidationService, JwtService jwtService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _signUpValidationService = signupValidationService;
            _jwtService = jwtService;
        }

        [HttpPost("signup")]
        public async Task<ActionResult<RbacResponse>> Signup([FromBody] SignupRequest signupRequest)
        {
            if (!ModelState.IsValid)
            {
                List<string> errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(RbacResponse.BadRequest("Validation falied", errors));
            }

            var businessValidation = await _signUpValidationService.ValidateSignupAsync(signupRequest);
            if (!businessValidation.IsValid)
            {
                return BadRequest(RbacResponse.BadRequest("Validation failed", businessValidation.Errors));
            }

            User user = new User
            {
                UserName = signupRequest.UserName,
                Email = signupRequest.Email,
                FirstName = signupRequest.FirstName,
                LastName = signupRequest.LastName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                AccessFailedCount = 0,
                LockoutEnabled = true,
            };

            IdentityResult result = await _userManager.CreateAsync(user, signupRequest.Password);

            if (!result.Succeeded)
            {
                List<string> errorMsg = result.Errors.Select(e => e.Description).ToList();
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Signup failed");
                foreach (string err in errorMsg)
                {
                    sb.AppendLine(err);
                }
                return BadRequest(RbacResponse.BadRequest(sb.ToString(), errorMsg));
            }

            IList<User> adminExists = await _userManager.GetUsersInRoleAsync("SUPER_ADMIN");

            if (adminExists.Count > 0)
            {
                await _userManager.AddToRoleAsync(user, "Junior Staff");
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "SUPER_ADMIN");
            }   
            return Ok(RbacResponse.Created(signupRequest, "Signup successfull"));
        }

        [HttpPost("signin")]
        public async Task<ActionResult<RbacResponse>> Signin([FromBody] SigninRequest signinRequest)
        {
            if (!ModelState.IsValid)
            {
                List<string> errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(RbacResponse.BadRequest("Validation failed", errors));
            }
            string? clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();

            User? user = await _userManager.FindByEmailAsync(signinRequest.EmailOrUsername) ??
                        await _userManager.FindByNameAsync(signinRequest.EmailOrUsername);

            if (user == null)
            {
                return BadRequest(RbacResponse.BadRequest($"Invalid credentials. Signin attempt with non-existent user: {signinRequest.EmailOrUsername} from IP: {clientIp}"));
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                return BadRequest(RbacResponse.BadRequest("Account is locked out. Please try again later."));
            }

            if (!user.IsActive)
            {
                return BadRequest(RbacResponse.BadRequest("Account is deactivated. Please contact administrator."));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, signinRequest.Password, true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    return BadRequest(RbacResponse.BadRequest("Account locked out due to multiple failed attempts."));
                }
                int failedCount = await _userManager.GetAccessFailedCountAsync(user);
                int maxAttempts = _userManager.Options.Lockout.MaxFailedAccessAttempts;
                int attemptsLeft = maxAttempts - failedCount;
                return BadRequest(RbacResponse.BadRequest($"Invalid credentials. User {signinRequest.EmailOrUsername} has {attemptsLeft} attempts remaining before lockout"));
            }

            if (_userManager.Options.SignIn.RequireConfirmedEmail && !user.EmailConfirmed)
            {
                return BadRequest(RbacResponse.BadRequest("Please confirm your email address before signing in."));
            }

            var authResponse = await GenerateAuthResponseAsync(user);

            user.LastLoginAt = DateTime.UtcNow;
            user.LastLoginIp = clientIp;
            await _userManager.UpdateAsync(user);

            return Ok(RbacResponse.Ok(authResponse, "Sign in successful"));
        }

        [HttpPost("signout")]
        [Authorize]
        public async Task<ActionResult<RbacResponse>> Signout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(RbacResponse.BadRequest("User not found"));
            }

            var refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                .ToListAsync();

            foreach (var token in refreshTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(RbacResponse.Ok(null, "Signed out successfully"));
        }

        [HttpPost("signout-all")]
        [Authorize]
        public async Task<ActionResult<RbacResponse>> SignoutAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(RbacResponse.BadRequest("User not found"));
            }

            var refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                .ToListAsync();

            foreach (var token in refreshTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(RbacResponse.Ok(null, "Signed out from all devices successfully"));
        }

        private async Task<AuthResponse> GenerateAuthResponseAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("FirstName", user.FirstName ?? ""),
                new Claim("LastName", user.LastName ?? "")
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var accessToken = _jwtService.GenerateAccessToken(claims);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var refreshTokenExpiration = _jwtService.GetRefreshTokenExpiration();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = refreshTokenExpiration,
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Ensure HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = refreshTokenExpiration
            };

            Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);

            return new AuthResponse
            {
                AccessToken = accessToken,
                User = new UserInfo
                {
                    Id = user.Id,
                    UserName = user.UserName ?? "",
                    Email = user.Email ?? "",
                    FirstName = user.FirstName ?? "",
                    LastName = user.LastName ?? "",
                    Roles = roles.ToList()
                }
            };
        }
    }
}
