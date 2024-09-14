namespace Mio_Rest_Api.DTO
{
    public class HECStatutDTO
    {
        public int Id { get; set; } // Identifiant du statut
        public string Statut { get; set; } = string.Empty; // Description du statut
        public string CreatedAt { get; set; } = string.Empty; // Timestamp format string (parsing via ParseExact)
        public string CreatedBy { get; set; } = string.Empty; // Auteur du statut (Client, Manager, etc.)
        public int ReservationId { get; set; } // Lien vers la réservation concernée
    }
}
