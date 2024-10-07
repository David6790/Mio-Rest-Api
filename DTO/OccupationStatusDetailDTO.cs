namespace Mio_Rest_Api.DTO
{
    public class OccupationStatusDetailDTO
    {
        public DateOnly DateOfEffect { get; set; }
        public string OccStatusDiner { get; set; } = string.Empty;
        public string OccStatusMidi { get; set; } = string.Empty;
        public List<string>? TimeSlots { get; set; }
    }
}
