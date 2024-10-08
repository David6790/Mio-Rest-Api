using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mio_Rest_Api.Data;
using Mio_Rest_Api.DTO;
using Mio_Rest_Api.Entities;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public interface IAuthService
{
    Task<(string Token, UserEntity User)?> Authenticate(LoginDTO loginDto);
    string GenerateJwtToken(UserEntity user);
    Task<UserEntity?> Signup(SignupDTO signupDto);
    Task<List<UserDTO>> GetAllUsers();
    Task<bool> UpdateUserRole(string email, string newRole); // Méthode pour modifier le rôle
    Task<bool> DeleteUser(string email); // Méthode pour supprimer l'utilisateur
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

    public async Task<(string Token, UserEntity User)?> Authenticate(LoginDTO loginDto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == loginDto.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
        {
            return null;
        }

        var token = GenerateJwtToken(user);
        return (token, user);
    }
    public async Task<UserEntity?> Signup(SignupDTO signupDto)
    {
        // Vérifier si l'utilisateur ou l'email existe déjà
        if (await _context.Users.AnyAsync(u => u.Username == signupDto.Username || u.Email == signupDto.Email))
        {
            return null; // Utilisateur ou email déjà existant
        }

        // Hacher le mot de passe
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(signupDto.Password);

        // Créer un nouvel utilisateur avec le rôle en majuscule
        var user = new UserEntity
        {
            Username = signupDto.Username,
            Password = hashedPassword,
            Email = signupDto.Email,
            Role = signupDto.Role.ToUpperInvariant(), // Convertir le rôle en majuscules
            Nom = signupDto.Nom,
            Prenom = signupDto.Prenom,
        };

        // Ajouter l'utilisateur à la base de données
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

    public async Task<List<UserDTO>> GetAllUsers()
    {
        var users = await _context.Users
            .Select(u => new UserDTO
            {
                Nom = u.Nom,
                Prenom = u.Prenom,
                Role = u.Role,
                Email = u.Email
            })
            .ToListAsync();

        return users;
    }



    // Méthode pour mettre à jour le rôle d'un utilisateur
    public async Task<bool> UpdateUserRole(string email, string newRole)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return false; // Utilisateur non trouvé
        }

        // Convertir le rôle en majuscule avant de le sauvegarder en base
        user.Role = newRole.ToUpperInvariant();
        await _context.SaveChangesAsync();
        return true;
    }

    // Méthode pour supprimer un utilisateur par email
    public async Task<bool> DeleteUser(string email)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return false; // Utilisateur non trouvé
        }

        // Empêcher la suppression si l'utilisateur est le créateur de l'application
        if (user.Email == "david.lb90@gmail.com" && user.Role == "ADMIN")
        {
            throw new InvalidOperationException("Impossible de supprimer le créateur de l'application.");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
}




