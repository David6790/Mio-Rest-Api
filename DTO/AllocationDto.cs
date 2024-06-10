namespace Mio_Rest_Api.DTO
{
    public class AllocationDto
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public string Date { get; set; } // Date au format chaîne de caractères
        public string Period { get; set; } // "Midi" ou "Soir"
        public ReservationDTO Reservation { get; set; } // Ajout de la propriété Reservation
        public TableDto Table { get; set; } // Ajout de la propriété Table
        public string IsMultiTable { get; set; }
    }
}
