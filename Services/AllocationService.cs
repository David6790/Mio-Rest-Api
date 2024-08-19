using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Mio_Rest_Api.Data;
using Mio_Rest_Api.DTO;
using Mio_Rest_Api.Entities;

public interface IAllocationService
{
    void CreateAllocation(CreateAllocationRequestDto requestDto);
    List<AllocationDto> GetAllocations(DateOnly date, string period);
    void DeleteAllocations(int reservationId); // Méthode pour supprimer les allocations
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

        // Récupérer la réservation associée pour obtenir timeResa et FreeTable21
        var reservation = _context.Reservations
            .FirstOrDefault(r => r.Id == requestDto.ReservationId);

        if (reservation == null)
        {
            throw new InvalidOperationException("Réservation introuvable.");
        }

        // Utilisation du timeResa de la réservation
        TimeOnly timeResa = reservation.TimeResa;

        foreach (var tableId in requestDto.TableId)
        {
            // Vérification des allocations existantes pour la même table, date et période
            var existingAllocations = _context.Allocations
                .Include(a => a.Reservation)
                .Where(a => a.TableId == tableId && a.Date == date && a.Period == requestDto.Period)
                .ToList();

            // Vérification du nombre d'allocations existantes pour cette table
            if (existingAllocations.Count >= 2)
            {
                throw new InvalidOperationException("La table a déjà atteint la limite d'allocations pour la date et la période spécifiées.");
            }

            foreach (var allocation in existingAllocations)
            {
                var existingReservation = allocation.Reservation;
                if (existingReservation != null)
                {
                    // Si la table est occupée sans libération à 21h et que la nouvelle réservation est avant 21h
                    TimeOnly freeTableThreshold = new TimeOnly(21, 0); // 21h00
                    if (existingReservation.FreeTable21 != "O" || timeResa < freeTableThreshold)
                    {
                        throw new InvalidOperationException("La table n'est libre qu'à partir de 21h. Vous ne pouvez pas allouer une réservation arrivant avant 21h");
                    }
                }
            }

            // Si aucune collision, créer la nouvelle allocation
            var newAllocation = new Allocation
            {
                ReservationId = requestDto.ReservationId,
                TableId = tableId,
                Date = date,
                Period = requestDto.Period,
                IsMultiTable = isMultiTable ? "Y" : "N"
            };

            _context.Allocations.Add(newAllocation);
        }

        // Mettre à jour la réservation associée pour indiquer qu'elle est placée
        reservation.Placed = "O";
        _context.Reservations.Update(reservation);

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



    public void DeleteAllocations(int reservationId)
    {
        var reservation = _context.Reservations.FirstOrDefault(r => r.Id == reservationId);

        // Vérifier si la réservation est placée
        if (reservation != null && reservation.Placed == "O")
        {
            var allocations = _context.Allocations.Where(a => a.ReservationId == reservationId);

            if (allocations.Any())
            {
                _context.Allocations.RemoveRange(allocations);
                _context.SaveChanges();
            }

            // Mettre à jour l'état Placed à "N" après la suppression des allocations
            reservation.Placed = "N";
            _context.Reservations.Update(reservation);
            _context.SaveChanges();
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
