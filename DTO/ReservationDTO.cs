namespace Mio_Rest_Api.DTO
{
    public class ReservationDTO
    {
        public string DateResa { get; set; }=string.Empty;
        public string TimeResa { get; set; } = string.Empty;
        public int NumberOfGuest { get; set; }
        public string? Comment { get; set; }

       
        public string ClientName { get; set; }=string.Empty;
        public string ClientPrenom { get; set; } = string.Empty;
        public string? ClientTelephone { get; set; } 
        public string? ClientEmail { get; set; }
    }
}
