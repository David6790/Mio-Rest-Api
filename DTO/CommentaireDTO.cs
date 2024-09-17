namespace Mio_Rest_Api.DTO
{
    public class CommentaireDTO
    {
        public int Id { get; set; } // Identifiant du commentaire
        public string Message { get; set; } = string.Empty; // Contenu du commentaire
        public string Auteur { get; set; } = string.Empty; // Auteur du commentaire (Client ou Restaurant)
        public int ReservationId { get; set; } // Lien vers la réservation concernée
    }
}
