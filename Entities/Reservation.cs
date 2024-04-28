namespace Mio_Rest_Api.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public int IdClient { get; set; }
        public DateOnly DateResa { get; set; }
        public TimeOnly TimeResa { get; set; }
        public int NumberOfGuest { get; set; }
        public DateTime CreaTimeStamp { get; set; }
        public DateTime? UpdateTimeStamp { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
        public string? Comment { get; set; }
        public string Placed { get; set; } = "N";
        public string IsPowerUser { get; set; } = "N";
        public string Status { get; set; } = "P";
        public string OccupationStatusOnBook { get; set; } = "RAS";
    }


    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string? Telephone { get; set; } 
        public string? Email { get; set; }
        public string FreeTable21 { get; set; } = string.Empty;
    }

    public class OccupationStatus
    {
        public int Id { get; set; }
        public DateTime DateOfEffect { get; set; }
        public string OccStatus { get; set; } = "RAS";

    }
}
