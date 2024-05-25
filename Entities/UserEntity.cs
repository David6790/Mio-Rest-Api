namespace Mio_Rest_Api.Entities
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty ;
        public string Prenom { get; set; } = string .Empty ;
    }
}
