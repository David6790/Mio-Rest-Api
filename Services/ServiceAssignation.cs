using Microsoft.EntityFrameworkCore;
using Mio_Rest_Api.Data;
using Mio_Rest_Api.DTO;
using Mio_Rest_Api.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mio_Rest_Api.Services

{

    public interface IServiceAssignation
    {
        Task<List<Assignation>> AddAssignations(AssignationRequestDto assignationRequestDto);
        Task<IEnumerable<Assignation>> GetAssignations();
        Task<IEnumerable<Assignation>> GetAssignationsByPeriod(DateOnly date, string period);
        Task<Assignation> GetAssignation(int id);
        Task<IEnumerable<TableDto>> GetAvailableTables(DateOnly date, string period);
    }
    public class ServiceAssignation : IServiceAssignation
    {
        private readonly ContextApplication _context;

        public ServiceAssignation(ContextApplication context)
        {
            _context = context;
        }

        public async Task<List<Assignation>> AddAssignations(AssignationRequestDto assignationRequestDto)
        {
            var reservation = await _context.Reservations.FindAsync(assignationRequestDto.ReservationId);
            if (reservation == null)
            {
                throw new KeyNotFoundException($"Reservation with ID {assignationRequestDto.ReservationId} not found.");
            }

            var tableIds = assignationRequestDto.TableAssignations.Select(t => t.TableId).ToList();
            var tables = await _context.Tables.Where(t => tableIds.Contains(t.Id)).ToListAsync();

            if (tables.Count != tableIds.Count)
            {
                throw new KeyNotFoundException("One or more tables not found.");
            }

            var assignations = assignationRequestDto.TableAssignations.Select(ta => new Assignation
            {
                ReservationId = assignationRequestDto.ReservationId,
                TableId = ta.TableId,
                Date = assignationRequestDto.Date,
                Periode = DeterminePeriode(ta.HeureDebut),
                HeureDebut = ta.HeureDebut,
                HeureFin = ta.HeureFin ?? DetermineHeureFin(reservation, ta.HeureDebut)
            }).ToList();

            _context.Assignations.AddRange(assignations);
            await _context.SaveChangesAsync();

            return assignations;
        }

        public async Task<IEnumerable<Assignation>> GetAssignations()
        {
            return await _context.Assignations
                .Include(a => a.Reservation)
                .Include(a => a.Table)
                .ToListAsync();
        }

        public async Task<IEnumerable<Assignation>> GetAssignationsByPeriod(DateOnly date, string period)
        {
            return await _context.Assignations
                .Include(a => a.Reservation)
                .ThenInclude(r => r.Client)
                .Include(a => a.Table)
                .Where(a => a.Date == date && a.Periode == period)
                .ToListAsync();
        }

        public async Task<Assignation> GetAssignation(int id)
        {
            var assignation = await _context.Assignations
                .Include(a => a.Reservation)
                .Include(a => a.Table)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assignation == null)
            {
                throw new KeyNotFoundException($"Assignation with ID {id} not found.");
            }

            return assignation;
        }

        public async Task<IEnumerable<TableDto>> GetAvailableTables(DateOnly date, string period)
        {
            var assignedTableIds = await _context.Assignations
                .Where(a => a.Date == date && a.Periode == period)
                .Select(a => a.TableId)
                .ToListAsync();

            return await _context.Tables
                .Where(t => !assignedTableIds.Contains(t.Id))
                .Select(t => new TableDto
                {
                    Id = t.Id,
                    NumeroTable = t.NumeroTable
                })
                .ToListAsync();
        }


        private string DeterminePeriode(TimeOnly heureDebut)
        {
            return heureDebut < new TimeOnly(14, 0) ? "midi" : "soir";
        }

        private TimeOnly DetermineHeureFin(ReservationEntity reservation, TimeOnly heureDebut)
        {
            if (reservation.FreeTable21 == "Y" || reservation.FreeTable21 == "true" || reservation.FreeTable21 == "1")
            {
                return new TimeOnly(21, 0);
            }
            return heureDebut.AddHours(2);
        }
    }
}
