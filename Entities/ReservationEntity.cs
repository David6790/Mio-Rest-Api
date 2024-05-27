namespace Mio_Rest_Api.Entities
{
    public class ReservationEntity
    {
        public int Id { get; set; }
        public int IdClient { get; set; }
        public DateOnly DateResa { get; set; }
        public TimeOnly TimeResa { get; set; }
        public int NumberOfGuest { get; set; }
        public DateTime CreaTimeStamp { get; set; } = DateTime.Now;
        public DateTime? UpdateTimeStamp { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
        public string? Comment { get; set; }
        public string Placed { get; set; } = "N";
        public string IsPowerUser { get; set; } = "N";
        public string Status { get; set; } = "P";
        public string OccupationStatusOnBook { get; set; } = "RAS";
        public string FreeTable21 { get; set; } = string.Empty;

        // Propriété de navigation pour le client
        public virtual Client Client { get; set; } = null!;

        // Propriété de navigation pour les assignations
        public virtual ICollection<Assignation> Assignations { get; set; } = new List<Assignation>();
    }

    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string? Telephone { get; set; }
        public string? Email { get; set; }
        public int NumberOfReservation { get; set; } = 1;

        // Propriété de navigation pour les réservations
        public virtual ICollection<ReservationEntity> Reservations { get; set; } = new List<ReservationEntity>();
    }

    public class Table
    {
        public int Id { get; set; }
        public int NumeroTable { get; set; }

        // Propriété de navigation pour les assignations
        public virtual ICollection<Assignation> Assignations { get; set; } = new List<Assignation>();
    }

    public class Assignation
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }

        // Propriété de navigation pour la réservation
        public virtual ReservationEntity Reservation { get; set; } = null!;

        public int TableId { get; set; }

        // Propriété de navigation pour la table
        public virtual Table Table { get; set; } = null!;

        public DateOnly Date { get; set; }
        public string Periode { get; set; } = string.Empty;
        public TimeOnly HeureDebut { get; set; }
        public TimeOnly HeureFin { get; set; }
    }

    public class OccupationStatus
    {
        public int Id { get; set; }
        public DateOnly DateOfEffect { get; set; }
        public string OccStatus { get; set; } = "RAS";
    }
}
