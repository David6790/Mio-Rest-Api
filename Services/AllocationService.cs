using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Mio_Rest_Api.Data;
using Mio_Rest_Api.DTO;
using Mio_Rest_Api.Entities;

public interface IAllocationService
{
    void CreateAllocation(CreateAllocationRequestDto requestDto);
    List<AllocationDto> GetAllocations(DateOnly date, string period);
    void DeleteAllocations(int reservationId);
    void ChangeAllocation(ChangeAllocationRequestDto requestDto);
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

        // Récupérer la réservation associée pour obtenir timeResa et FreeTable21/FreeTable1330
        var reservation = _context.Reservations
            .FirstOrDefault(r => r.Id == requestDto.ReservationId);

        if (reservation == null)
        {
            throw new InvalidOperationException("Réservation introuvable.");
        }

        // Utilisation du timeResa de la réservation
        TimeOnly timeResa = reservation.TimeResa;

        // Séparation de la logique entre midi et soir
        if (requestDto.Period == "midi")
        {
            TimeOnly freeTableThreshold = new TimeOnly(13, 30); // 13h30 pour le midi

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
                        bool isExistingReservationAfter1330 = existingReservation.TimeResa >= freeTableThreshold;

                        if (timeResa < freeTableThreshold)
                        {
                            if (isExistingReservationAfter1330)
                            {
                                if (reservation.FreeTable1330 != "O")
                                {
                                    throw new InvalidOperationException("La table est déjà réservée après 13h30, vous ne pouvez pas ajouter cette réservation.");
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException("La table n'est pas libre pour une réservation avant 13h30.");
                            }
                        }
                        else
                        {
                            if (!isExistingReservationAfter1330 && existingReservation.FreeTable1330 != "O")
                            {
                                throw new InvalidOperationException("La table est déjà réservée pour une période qui se chevauche avec votre réservation.");
                            }
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
        }
        else if (requestDto.Period == "soir")
        {
            TimeOnly freeTableThreshold = new TimeOnly(21, 0); // 21h00 pour le soir

            foreach (var tableId in requestDto.TableId)
            {
                var existingAllocations = _context.Allocations
                    .Include(a => a.Reservation)
                    .Where(a => a.TableId == tableId && a.Date == date && a.Period == requestDto.Period)
                    .ToList();

                if (existingAllocations.Count >= 2)
                {
                    throw new InvalidOperationException("La table a déjà atteint la limite d'allocations pour la date et la période spécifiées.");
                }

                foreach (var allocation in existingAllocations)
                {
                    var existingReservation = allocation.Reservation;
                    if (existingReservation != null)
                    {
                        bool isExistingReservationAfter21h = existingReservation.TimeResa >= freeTableThreshold;

                        if (timeResa < freeTableThreshold)
                        {
                            if (isExistingReservationAfter21h)
                            {
                                if (reservation.FreeTable21 != "O")
                                {
                                    throw new InvalidOperationException("La table est déjà réservée après 21h, vous ne pouvez pas ajouter cette réservation.");
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException("La table n'est pas libre pour une réservation avant 21h.");
                            }
                        }
                        else
                        {
                            if (!isExistingReservationAfter21h && existingReservation.FreeTable21 != "O")
                            {
                                throw new InvalidOperationException("La table est déjà réservée pour une période qui se chevauche avec votre réservation.");
                            }
                        }
                    }
                }

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
        }

        // Mise à jour de la réservation associée pour indiquer qu'elle est placée
        reservation.Placed = "O";
        _context.Reservations.Update(reservation);

        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
            {
                throw new InvalidOperationException("Une des tables est déjà allouée pour la date et la période spécifiées.");
            }
            throw;
        }
    }

    public void DeleteAllocations(int reservationId)
    {
        var reservation = _context.Reservations.FirstOrDefault(r => r.Id == reservationId);

        if (reservation != null && reservation.Placed == "O")
        {
            var allocations = _context.Allocations.Where(a => a.ReservationId == reservationId);

            if (allocations.Any())
            {
                _context.Allocations.RemoveRange(allocations);
                _context.SaveChanges();
            }

            reservation.Placed = "N";
            _context.Reservations.Update(reservation);
            _context.SaveChanges();
        }
    }

    public void ChangeAllocation(ChangeAllocationRequestDto requestDto)
    {
        var createRequestDto = new CreateAllocationRequestDto
        {
            ReservationId = requestDto.ReservationId,
            Date = requestDto.Date,
            Period = requestDto.Period,
            TableId = requestDto.NewTableIds
        };

        ValidateAllocation(createRequestDto);
        DeleteAllocations(requestDto.ReservationId);
        CreateAllocation(createRequestDto);
    }

    private void ValidateAllocation(CreateAllocationRequestDto requestDto)
    {
        if (!DateOnly.TryParse(requestDto.Date, out var date))
        {
            throw new ArgumentException("Format de date invalide.");
        }

        var reservation = _context.Reservations.FirstOrDefault(r => r.Id == requestDto.ReservationId);

        if (reservation == null)
        {
            throw new InvalidOperationException("Réservation introuvable.");
        }

        TimeOnly timeResa = reservation.TimeResa;

        if (requestDto.Period == "midi")
        {
            TimeOnly freeTableThreshold = new TimeOnly(13, 30); // 13h30 pour le midi

            foreach (var tableId in requestDto.TableId)
            {
                var existingAllocations = _context.Allocations
                    .Include(a => a.Reservation)
                    .Where(a => a.TableId == tableId && a.Date == date && a.Period == requestDto.Period)
                    .ToList();

                if (existingAllocations.Count >= 2)
                {
                    throw new InvalidOperationException($"La table {tableId} a déjà atteint la limite d'allocations pour la date et la période spécifiées.");
                }

                foreach (var allocation in existingAllocations)
                {
                    var existingReservation = allocation.Reservation;
                    if (existingReservation != null)
                    {
                        bool isExistingReservationAfter1330 = existingReservation.TimeResa >= freeTableThreshold;

                        if (timeResa < freeTableThreshold)
                        {
                            if (isExistingReservationAfter1330)
                            {
                                if (reservation.FreeTable1330 != "O")
                                {
                                    throw new InvalidOperationException("La table est déjà réservée après 13h30, vous ne pouvez pas ajouter cette réservation.");
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException("La table n'est pas libre pour une réservation avant 13h30.");
                            }
                        }
                        else
                        {
                            if (!isExistingReservationAfter1330 && existingReservation.FreeTable1330 != "O")
                            {
                                throw new InvalidOperationException("La table est déjà réservée pour une période qui se chevauche avec votre réservation.");
                            }
                        }
                    }
                }
            }
        }
        else if (requestDto.Period == "soir")
        {
            TimeOnly freeTableThreshold = new TimeOnly(21, 0); // 21h00 pour le soir

            foreach (var tableId in requestDto.TableId)
            {
                var existingAllocations = _context.Allocations
                    .Include(a => a.Reservation)
                    .Where(a => a.TableId == tableId && a.Date == date && a.Period == requestDto.Period)
                    .ToList();

                if (existingAllocations.Count >= 2)
                {
                    throw new InvalidOperationException($"La table {tableId} a déjà atteint la limite d'allocations pour la date et la période spécifiées.");
                }

                foreach (var allocation in existingAllocations)
                {
                    var existingReservation = allocation.Reservation;
                    if (existingReservation != null)
                    {
                        bool isExistingReservationAfter21h = existingReservation.TimeResa >= freeTableThreshold;

                        if (timeResa < freeTableThreshold)
                        {
                            if (isExistingReservationAfter21h)
                            {
                                if (reservation.FreeTable21 != "O")
                                {
                                    throw new InvalidOperationException("La table est déjà réservée après 21h, vous ne pouvez pas ajouter cette réservation.");
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException("La table n'est pas libre pour une réservation avant 21h.");
                            }
                        }
                        else
                        {
                            if (!isExistingReservationAfter21h && existingReservation.FreeTable21 != "O")
                            {
                                throw new InvalidOperationException("La table est déjà réservée pour une période qui se chevauche avec votre réservation.");
                            }
                        }
                    }
                }
            }
        }
    }

    public List<AllocationDto> GetAllocations(DateOnly date, string period)
    {
        period = period.ToUpper();

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
                OccupationStatusSoirOnBook = a.Reservation.OccupationStatusSoirOnBook,
                CreatedBy = a.Reservation.CreatedBy,
                FreeTable21 = a.Reservation.FreeTable21,
                FreeTable1330 = a.Reservation.FreeTable1330,
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
