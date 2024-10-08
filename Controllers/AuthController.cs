using Microsoft.AspNetCore.Mvc;
using Mio_Rest_Api.DTO;
using Mio_Rest_Api.Services;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        // Action pour récupérer la liste des utilisateurs
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _authService.GetAllUsers();
            if (users == null || users.Count == 0)
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }

        // Nouvelle action pour modifier le rôle d'un utilisateur
        [HttpPut("users/role")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateRoleDTO updateRoleDto)
        {
            var success = await _authService.UpdateUserRole(updateRoleDto.Email, updateRoleDto.NewRole);
            if (!success)
            {
                return NotFound("User not found.");
            }

            return Ok("User role updated successfully.");
        }

        // Nouvelle action pour supprimer un utilisateur par email
        [HttpDelete("users/{email}")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            try
            {
                var success = await _authService.DeleteUser(email);
                if (!success)
                {
                    return NotFound("User not found.");
                }
                return Ok("User deleted successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // Renvoyer un message d'erreur spécifique pour le créateur de l'application
            }
        }

    }
}
