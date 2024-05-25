using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mio_Rest_Api.Data;
using Mio_Rest_Api.DTO;
using Mio_Rest_Api.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Mio_Rest_Api.Services
{

    public interface IAuthService
    {
        Task<UserEntity?> Authenticate(LoginDTO loginDto);
        string GenerateJwtToken(UserEntity user);
        Task<UserEntity?> Signup(SignupDTO signupDto);
    }

    public class AuthService : IAuthService
    {
        private readonly ContextApplication _context;
        private readonly IConfiguration _configuration;

        public AuthService(ContextApplication context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<UserEntity?> Authenticate(LoginDTO loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == loginDto.Username && u.Password == loginDto.Password);
            return user;
        }

        public async Task<UserEntity?> Signup(SignupDTO signupDto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == signupDto.Username || u.Email == signupDto.Email))
            {
                return null; // Utilisateur ou email déjà existant
            }

            var user = new UserEntity
            {
                Username = signupDto.Username,
                Password = signupDto.Password,
                Email = signupDto.Email,
                Role = signupDto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public string GenerateJwtToken(UserEntity user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings.GetValue<string>("SecretKey"));

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        }),
                Expires = DateTime.UtcNow.AddDays(7), // Token valide pendant 7 jours
                Issuer = jwtSettings.GetValue<string>("Issuer"),
                Audience = jwtSettings.GetValue<string>("Audience"),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
