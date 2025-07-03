using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RBAC_API.Database;
using RBAC_API.Models;
using RBAC_API.Models.DTOs;
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
        private readonly IConfiguration _config;

        public AuthController(RbacContext context, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        [HttpPost("signup")]
        public async Task<ActionResult<RbacResponse>> Signup([FromBody] SignupRequest signupRequest)
        {
            RbacResponse res;

            if (!ModelState.IsValid)
            {
                List<string> errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                res = RbacResponse.BadRequest("Validation falied", errors);
                return BadRequest(res);
            }

            var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == signupRequest.UserName || u.Email == signupRequest.Email);

            if (existingUser != null)
            {
                string conflictField = existingUser.UserName == signupRequest.UserName ? "Username" : "Email";
                res = RbacResponse.Conflict($"{conflictField} already exists");
                return Conflict(res);
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
                res = RbacResponse.BadRequest(sb.ToString(), errorMsg);
                return BadRequest(res);
            }

            IList<User> adminExists = await _userManager.GetUsersInRoleAsync("Super Admin");

            if (adminExists.Count > 0)
            {
                await _userManager.AddToRoleAsync(user, "Junior Staff");
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Super Admin");
            }   
            res = RbacResponse.Created(signupRequest, "Signup successfull");
            return Ok(res);
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
