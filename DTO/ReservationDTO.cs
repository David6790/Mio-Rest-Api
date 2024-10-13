namespace Mio_Rest_Api.DTO
{
    public class ReservationDTO
    {
        public string DateResa { get; set; }=string.Empty;
        public string TimeResa { get; set; } = string.Empty;
        public int NumberOfGuest { get; set; }
        public string? Comment { get; set; }
        public string OccupationStatusMidiOnBook { get; set; } = string.Empty;
        public string OccupationStatusSoirOnBook { get; set; } = string.Empty;
        public string CreatedBy { get; set; }=string.Empty ;
        public string FreeTable21 { get; set; } = "N";
        public string FreeTable1330 { get; set; } = "N";
        public string UpdatedBy { get; set; } = string.Empty;
        public string CanceledBy { get; set; } = string.Empty;
        public string origin { get; set; } = string.Empty;
        public string DoubleConfirmation {  get; set; } = string.Empty; 
        public string Notifications {  get; set; } = string.Empty; 
        public string Status {  get; set; } = string.Empty;


        public string ClientName { get; set; }=string.Empty;
        public string ClientPrenom { get; set; } = string.Empty;
        public string? ClientTelephone { get; set; } 
        public string? ClientEmail { get; set; }
        
    }
}
