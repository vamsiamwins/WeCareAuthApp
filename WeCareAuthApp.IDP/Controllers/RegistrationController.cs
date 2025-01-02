using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;
using WeCareAuthApp.IDP;
using WeCareAuthApp.IDP.Models;

namespace WeCareAuthApp.IDP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly AuthDbContext _authcontext;
        private readonly UserDbContext _userDbContext;
        private readonly UserManager<User> _userManager;
        public RegistrationController(IMemoryCache cache, AuthDbContext context, UserDbContext userDbContext, UserManager<User> userManager)
        {
            _cache = cache;
            _authcontext = context;
            _userDbContext = userDbContext;
            _userManager = userManager;
        }

        [HttpPost("RegisterEmployee")]
        public async Task<IActionResult> RegisterEmployee([FromBody] RegistrationDTO request)
        {
            // Validate input
            if (string.IsNullOrEmpty(request.SessionKey))
            {
                return BadRequest(new { message = "SessionKey is required." });
            }

            //Retrieve email from session key
            if (!_cache.TryGetValue(request.SessionKey, out string email))
            {
                return Unauthorized(new { message = "Invalid or expired session key." });
            }

            // Create a new employee record
            var employee = new Employee
            {
                SSN = request.SSN,
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                LastName = request.LastName,
                PrimaryPhoneType = request.PhoneType,
                PrimaryEmailType = request.EmailType,
                OptInCommunication = request.OptInCommunication
            };

            // Save to database
            await _authcontext.Employees.AddAsync(employee);
            await _authcontext.SaveChangesAsync();

            return Ok(new { message = "Employee registered successfully." });
        }
    

    [HttpGet("GetCompanyName")]
        public async Task<IActionResult> GetSessionAsync([FromQuery] string companyEmail, [FromQuery] string companyPin)
        {
            // Validate input
            if (string.IsNullOrEmpty(companyEmail) || string.IsNullOrEmpty(companyPin))
            {
                return BadRequest(new { message = "Email and Pin are required." });
            }

            var company = await _authcontext.Companies
                .FirstOrDefaultAsync(c => c.CompanyEmail == companyEmail && c.CompanyPin == companyPin);

            if (company == null)
            {
                return Unauthorized(new { message = "Invalid email or pin." });
            }


            // Generate a session key
            var sessionKey = Guid.NewGuid().ToString();

            // Cache the session key for 5 minutes
            _cache.Set(sessionKey, companyEmail, TimeSpan.FromMinutes(5));

            // Return the response
            return Ok(new
            {
                companyName = company.CompanyName,
                sessionKey
            });
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateRequest request)
        {
            // Validate input
            if (string.IsNullOrEmpty(request.SessionKey) ||
                string.IsNullOrEmpty(request.Password) ||
                string.IsNullOrEmpty(request.ConfirmPassword))
            {
                return BadRequest(new { message = "SessionKey, Password, and ConfirmPassword are required." });
            }

            // Check if passwords match
            if (request.Password != request.ConfirmPassword)
            {
                return BadRequest(new { message = "Password and ConfirmPassword do not match." });
            }

            // Validate session key
            if (!_cache.TryGetValue(request.SessionKey, out string email))
            {
                return Unauthorized(new { message = "Invalid or expired session key." });
            }

            // Hash the password
            var hashedPassword = HashPassword(request.Password);

            // Create a new user
            var newUser = new User
            {
                Email = email,
                UserName = email // Typically UserName is the same as Email in most setups
            };

            // Create the user with the hashed password
            var result = await _userManager.CreateAsync(newUser, request.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Failed to create user.", errors = result.Errors });
            }

            return Ok(new { message = "User created successfully." });
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }

    public class UserCreateRequest
    {
        public string SessionKey { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
   
}

