using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Security.Policy;

namespace WeCareAuthApp.IDP.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        //private readonly TokenClient _tokenClient;
        private readonly IHttpClientFactory _httpClient;

        public AuthController(UserManager<User> userManager,  IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
           // _tokenClient = tokenClient;
            _httpClient = httpClientFactory;
        }
        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromBody] LoginModel model)
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
            var isValidPwd = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isValidPwd)
            {
                return Unauthorized("Invalid Password");
            }
            //var tokenReq = new PasswordTokenRequest
            //{
            //    Address = "https://localhost:5003/connect/token",
            //    ClientId = "testclientid",
            //    ClientSecret = "secret",
            //    GrantType = "password",
            //    Parameters = {
            //        {"scope", "openid profile apiscope1" },
            //        {"username", model.Email },
            //        {"password",model.Password } 
            //    }
            //};
            //var tokenResponse = await _tokenClient.RequestPasswordTokenAsync(tokenReq);
            //var tokenResponse = await _tokenClient.RequestTokenAsync("curl - X POST "https://localhost:5003/connect/token" - H "Content-Type: application/x-www-form-urlencoded" - d "client_id=testclientid" - d "client_secret=secret" - d "grant_type=password" - d "username=testuser" - d "password=testpassword" - d "scope=openid profile apiscope1"");

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

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
