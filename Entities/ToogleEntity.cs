namespace Mio_Rest_Api.Entities
{
    public class ToggleEntity
    {
        public int Id { get; set; }  // Identifiant unique du toggle
        public string Name { get; set; } = string.Empty;  // Nom du toggle, comme "Notification" ou autre
        public int NotificationCount { get; set; }  // Nombre de notifications actives (incrémenté/décrémenté)
        public DateTime LastUpdated { get; set; }  // Date et heure de la dernière modification
    }
}