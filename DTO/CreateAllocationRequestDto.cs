namespace Mio_Rest_Api.DTO
{
    public class CreateAllocationRequestDto
    {
        public string Date { get; set; } // Date au format chaîne de caractères (ex. "2024-06-08")
        public int ReservationId { get; set; }
        public List<int> TableId { get; set; }
        public string Period { get; set; } // "Midi" ou "Soir"
    }

}
