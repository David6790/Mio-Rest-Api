namespace Mio_Rest_Api.DTO
{
    public class AssignationRequestDto
    {
        public int ReservationId { get; set; }
        public DateOnly Date { get; set; }
        public List<TableAssignationDto> TableAssignations { get; set; } = new List<TableAssignationDto>();
    }

    public class TableAssignationDto
    {
        public int TableId { get; set; }
        public TimeOnly HeureDebut { get; set; }
        public TimeOnly? HeureFin { get; set; }
    }
}
