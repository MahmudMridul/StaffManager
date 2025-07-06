using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RBAC_API.Common;
using RBAC_API.Database;
using RBAC_API.Models;
using RBAC_API.Models.DTOs;
using RBAC_API.Servies;
using System.Net;
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

        public AuthController(RbacContext context, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config, SignupValidationService signupValidationService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _signUpValidationService = signupValidationService;
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

        // GET: api/<AuthController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AuthController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AuthController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AuthController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AuthController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
