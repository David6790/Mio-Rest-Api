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
        public DateTime? UpdateTimeStamp { get; set; }
        public DateTime? CanceledTimeStamp { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
        public string? CanceledBy { get; set; }
        public string? Comment { get; set; }
        public string Placed { get; set; } = "N";
        public string IsPowerUser { get; set; } = "N";
        public string Status { get; set; } = "P";
        public string OccupationStatusOnBook { get; set; } = "RAS";
        public string FreeTable21 { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string DoubleConfirmation { get; set; } = string.Empty;
        public string Notifications { get; set; } = string.Empty;

        // Propriété de navigation pour le client
        public virtual Client Client { get; set; } = null!;
        // Propriété de navigation pour les allocations
        public virtual ICollection<Allocation> Allocations { get; set; } = new List<Allocation>();
        // Propriété de navigation pour les statuts HEC
        public virtual ICollection<HECStatut> HECStatuts { get; set; } = new List<HECStatut>();
        // Propriété de navigation pour les commentaires
        public virtual ICollection<Commentaire> Commentaires { get; set; } = new List<Commentaire>();
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

    public class OccupationStatus
    {
        public int Id { get; set; }
        public DateOnly DateOfEffect { get; set; }
        public string OccStatus { get; set; } = "RAS";
    }

    public class TableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; } // Nombre de personnes que la table peut accueillir

        // Propriétés de navigation
        public virtual ICollection<Allocation> Allocations { get; set; } = new List<Allocation>();
    }

    public class Allocation
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public int TableId { get; set; }
        public DateOnly Date { get; set; }
        public string Period { get; set; } // "Midi" ou "Soir"
        public string IsMultiTable { get; set; } = "N";

        // Propriétés de navigation
        public virtual ReservationEntity Reservation { get; set; } = null!;
        public virtual TableEntity Table { get; set; } = null!;
    }

    // Entité HECStatut pour gérer les statuts de la timeline
    public class HECStatut
    {
        public int Id { get; set; } // Identifiant unique du statut
        public int ReservationId { get; set; } // Lien vers la réservation concernée
        public string Actions { get; set; } = string.Empty; // Description de l'action (ex: "RAS")
        public string Statut { get; set; } = string.Empty; // Description du statut (ex: "Réservation enregistrée")
        public string Libelle { get; set; } = string.Empty; // Description du statut (ex: "En attente de validation")
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Timestamp de la création
        public string CreatedBy { get; set; } = string.Empty; // Créé par (Client, Manager, etc.)

        // Propriété de navigation vers ReservationEntity
        public virtual ReservationEntity Reservation { get; set; } = null!;
    }

    // Entité Commentaire pour les échanges entre client et restaurant
    public class Commentaire
    {
        public int Id { get; set; } // Identifiant unique du commentaire
        public int ReservationId { get; set; } // Lien vers la réservation concernée
        public string Message { get; set; } = string.Empty; // Contenu du message
        public string Auteur { get; set; } = string.Empty; // Auteur du message (Client ou Restaurant)
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Timestamp de création du commentaire

        // Propriété de navigation vers ReservationEntity
        public virtual ReservationEntity Reservation { get; set; } = null!;
    }
}
