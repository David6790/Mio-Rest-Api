namespace Mio_Rest_Api.DTO
{
    public class OccupationStatusDetailDTO
    {
        public DateOnly DateOfEffect { get; set; }
        public string OccStatus { get; set; } = string.Empty;
        public List<string>? TimeSlots { get; set; }
    }
}
