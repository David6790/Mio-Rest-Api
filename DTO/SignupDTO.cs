﻿namespace Mio_Rest_Api.DTO
{
    public class SignupDTO
    {
        public string Username { get; set; }=string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // Rôle par défaut
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
    }
}
