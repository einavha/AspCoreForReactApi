using Microsoft.AspNetCore.Mvc;

namespace ReactApp1.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticateController : ControllerBase
    {
        public class LoginRequest
        {
            public string? Username { get; set; }
            public string? Password { get; set; }
        }

        public class LoginResponse
        {
            public string? Token { get; set; }
            public string? Message { get; set; }
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Post(LoginRequest request)
        {
            // For demonstration purposes, we will use hardcoded credentials.
            // In a real application, you should validate against a database or other user store.
            if (request.Username == "admin" && request.Password == "password")
            {
                // Generate a fake token for demonstration. In a real application, use JWT or similar.
                var token = Convert.ToBase64String(System.Guid.NewGuid().ToByteArray());
                return Ok(new LoginResponse { Token = token, Message = "Login successful" });
            }
            else
            {
                return Unauthorized(new LoginResponse { Message = "Invalid username or password" });
            }
        }        
    }
}
