using Microsoft.AspNetCore.Mvc;
using Mio_Rest_Api.DTO;
using Mio_Rest_Api.Services;
using System.Threading.Tasks;

namespace Mio_Rest_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var result = await _authService.Authenticate(loginDto);
            if (result == null)
            {
                return Unauthorized();
            }

            var (token, user) = result.Value;
            return Ok(new
            {
                token,
                user = new
                {
                    user.Username,
                    user.Email,
                    user.Role
                }
            });
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupDTO signupDto)
        {
            var user = await _authService.Signup(signupDto);
            if (user == null)
            {
                return BadRequest("Username or email already exists.");
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token });
        }
    }
}
