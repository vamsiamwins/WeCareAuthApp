using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Security.Policy;
using Microsoft.Extensions.Caching.Memory;

namespace WeCareAuthApp.IDP.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        //private readonly TokenClient _tokenClient;
        private readonly IHttpClientFactory _httpClient;
        private readonly IMemoryCache _cache;

        public AuthController(UserManager<User> userManager,  IHttpClientFactory httpClientFactory, IMemoryCache cache)
        {
            _userManager = userManager;
           // _tokenClient = tokenClient;
            _httpClient = httpClientFactory;
            _cache = cache;
        }

        [HttpPost("validate-email")]
        public async Task<IActionResult> ValidateEmail([FromBody] EmailModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request");
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized("User Not Found");
            }
            var cacheKey = Guid.NewGuid().ToString();

            try
            {
                _cache.Set(cacheKey, user.Email, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error while caching session: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }

            return Ok(new { Message = "Email validated and session created.", SessionKey = cacheKey });

        }
        [HttpPost("validate-password")]
        public async Task<IActionResult> ValidatePassword([FromBody] PasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request");
            }

            if(!_cache.TryGetValue(model.SessionKey, out string email) || string.IsNullOrEmpty(email))
            {
                return Unauthorized("Session Expired");
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) {
                return Unauthorized("Invalid Email");
            }
            var isValidPwd = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isValidPwd)
            {
                return Unauthorized("Invalid Password");
            }
            
            var tokenUrl = "https://localhost:5003/connect/token";
            var client = _httpClient.CreateClient();

                var reqContent = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("client_id", "testclientid"),
                new KeyValuePair<string, string>("client_secret", "secret"),
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", "testuser"),
                new KeyValuePair<string, string>("password", "testpassword"),
                new KeyValuePair<string, string>("scope", "openid profile apiscope1"),
                });

            var response = await client.PostAsync(tokenUrl, reqContent);
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(new { error = "Token request failed.", details = await response.Content.ReadAsStringAsync() });
            }
            var tokenResponse = await response.Content.ReadAsStringAsync();
            return Content(tokenResponse, "application/json");
        }
    }
    public class EmailModel
    {
        public string Email { get; set; }
    }
    public class PasswordModel
    {
        public string SessionKey { get; set; }
        public string Password { get; set; }
    }
}
