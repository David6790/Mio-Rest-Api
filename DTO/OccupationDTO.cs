namespace Mio_Rest_Api.DTO
{
    public class OccupationDTO
    {
        
        public string DateOfEffect { get; set; } = string.Empty;
        public string OccStatus { get; set; } = string.Empty;
        public List<TimeSlotDTO> TimeSlots { get; set; } = new List<TimeSlotDTO>();
    }

    public class TimeSlotDTO
    {
        public string Slot { get; set; } = string.Empty ;
    }
}
