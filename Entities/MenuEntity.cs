namespace Mio_Rest_Api.Entities
{
    public class MenuEntity
    {
        public int Id { get; set; } 
        public DateOnly Date {  get; set; }
        public string Entree { get; set; } = string.Empty;
        public string Plat { get; set; } = string.Empty;
        public string? Cheesecake { get; set; }
        public string? DessertJour { get; set; }
    }
}
