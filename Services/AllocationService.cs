using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Mio_Rest_Api.Data;
using Mio_Rest_Api.DTO;
using Mio_Rest_Api.Entities;

namespace Mio_Rest_Api.Services
{

    public interface IAllocationService
    {
        void CreateAllocation(CreateAllocationRequestDto requestDto);
        List<AllocationDto> GetAllocations(DateOnly date, string period);
    }

    public class AllocationService : IAllocationService
    {
        private readonly ContextApplication _context;

        public AllocationService(ContextApplication context)
        {
            _context = context;
        }

        public void CreateAllocation(CreateAllocationRequestDto requestDto)
        {
            // Conversion de la date au format DateOnly
            if (!DateOnly.TryParse(requestDto.Date, out var date))
            {
                throw new ArgumentException("Format de date invalide.");
            }

            bool isMultiTable = requestDto.TableId.Count > 1;

            foreach (var tableId in requestDto.TableId)
            {
                var allocation = new Allocation
                {
                    ReservationId = requestDto.ReservationId,
                    TableId = tableId,
                    
                    Date = date,
                    Period = requestDto.Period,
                    IsMultiTable = isMultiTable ? "Y" : "N"
                };

                _context.Allocations.Add(allocation);
                var reservation = _context.Reservations
                    .FirstOrDefault(r => r.Id == requestDto.ReservationId);
                if (reservation != null)
                {
                    reservation.Placed = "O";
                    _context.SaveChanges();
                }
            }

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601) // Code d'erreur spécifique à SQL Server pour une violation de contrainte unique
                {
                    throw new InvalidOperationException("Une des tables est déjà allouée pour la date et la période spécifiées.");
                }
                throw;
            }
        }




        public List<AllocationDto> GetAllocations(DateOnly date, string period)
        {
            period = period.ToUpper(); // Normaliser la période pour la comparaison

            var allocations = _context.Allocations
                .Include(a => a.Reservation)
                    .ThenInclude(r => r.Client)
                .Include(a => a.Table)
                .Where(a => a.Date == date && a.Period.ToUpper() == period)
                .ToList();

            var allocationDtos = allocations.Select(a => new AllocationDto
            {
                Id = a.Id,
                ReservationId = a.ReservationId,
                Date = a.Date.ToString("yyyy-MM-dd"),
                Period = a.Period,
                IsMultiTable = a.IsMultiTable,
                Reservation = new ReservationDTO
                {
                    DateResa = a.Reservation.DateResa.ToString("yyyy-MM-dd"),
                    TimeResa = a.Reservation.TimeResa.ToString(),
                    NumberOfGuest = a.Reservation.NumberOfGuest,
                    Comment = a.Reservation.Comment,
                    OccupationStatusOnBook = a.Reservation.OccupationStatusOnBook,
                    CreatedBy = a.Reservation.CreatedBy,
                    FreeTable21 = a.Reservation.FreeTable21,
                    UpdatedBy = a.Reservation.UpdatedBy,
                    ClientName = a.Reservation.Client.Name,
                    ClientPrenom = a.Reservation.Client.Prenom,
                    ClientTelephone = a.Reservation.Client.Telephone,
                    ClientEmail = a.Reservation.Client.Email
                },
                Table = new TableDto
                {
                    Id = a.Table.Id,
                    Name = a.Table.Name,
                    Capacity = a.Table.Capacity
                }
            }).ToList();

            return allocationDtos;
        }

    }


}
