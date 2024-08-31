namespace Mio_Rest_Api.DTO
{
    public class ChangeAllocationRequestDto
    {
        public int ReservationId { get; set; }
        public string Date { get; set; } // Date au format chaîne de caractères (ex. "2024-06-08")
        public string Period { get; set; }
        public List<int> NewTableIds { get; set; } // Nouvelle liste des IDs de tables
    }

}
