using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Mio_Rest_Api.Data;
using Mio_Rest_Api.DTO;
using Mio_Rest_Api.Entities;

namespace Mio_Rest_Api.Services
{
    public interface IServiceReservation
    {
        Task<List<ReservationEntity>> GetAllReservations();
        Task<List<ReservationEntity>> GetFuturReservations();
        Task<ReservationEntity?> GetReservation(int id);
        Task<ReservationEntity> CreateReservation(ReservationDTO reservationDTO);
        Task<List<ReservationEntity>> GetReservationsByDate(string date);
        Task<ReservationEntity?> UpdateReservation(int id, ReservationDTO reservationDTO);
        Task<ReservationEntity?> ValidateReservation(int id);
        Task<ReservationEntity?> AnnulerReservation(int id, string u);
        Task<ReservationEntity?> RefuserReservation(int id, string u);
    }

    public class ServiceReservations : IServiceReservation
    {
        private readonly ContextApplication _contexte;
        private readonly IAllocationService _allocationService;

        // Constructeur pour instancier le db_context et le service d'allocation
        public ServiceReservations(ContextApplication contexte, IAllocationService allocationService)
        {
            _contexte = contexte;
            _allocationService = allocationService;
        }

        public async Task<List<ReservationEntity>> GetAllReservations()
        {
            return await _contexte.Reservations.Include(r => r.Client).ToListAsync();
        }

        public async Task<List<ReservationEntity>> GetFuturReservations()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            return await _contexte.Reservations.Include(r => r.Client).Where(r => r.DateResa >= today).OrderByDescending(r => r.CreaTimeStamp).ToListAsync();
        }

        public async Task<ReservationEntity?> GetReservation(int id)
        {
            return await _contexte.Reservations
                .Include(r => r.Client)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<ReservationEntity>> GetReservationsByDate(string date)
        {
            return await _contexte.Reservations
                .Include(r => r.Client)
                .Where(r => r.DateResa == DateOnly.ParseExact(date, "yyyy-MM-dd"))
                .OrderByDescending(r => r.CreaTimeStamp)
                .ToListAsync();
        }

        public async Task<ReservationEntity> CreateReservation(ReservationDTO reservationDTO)
        {
            Client? client = await _contexte.Clients.FirstOrDefaultAsync(c =>
                c.Name == reservationDTO.ClientName && c.Prenom == reservationDTO.ClientPrenom && c.Telephone == reservationDTO.ClientTelephone
            );

            if (client == null)
            {
                client = new Client
                {
                    Name = reservationDTO.ClientName,
                    Prenom = reservationDTO.ClientPrenom,
                    Telephone = reservationDTO.ClientTelephone,
                    Email = reservationDTO.ClientEmail,
                    NumberOfReservation = 1
                };
                _contexte.Clients.Add(client);
            }
            else
            {
                client.NumberOfReservation += 1;
            }
            await _contexte.SaveChangesAsync();

            if ((reservationDTO.OccupationStatusOnBook == "FreeTable21" || reservationDTO.OccupationStatusOnBook == "Service2Complet") && reservationDTO.TimeResa == "19:00")
            {
                reservationDTO.FreeTable21 = "O";
            }

            ReservationEntity reservation = new ReservationEntity
            {
                IdClient = client.Id,
                DateResa = DateOnly.ParseExact(reservationDTO.DateResa, "yyyy-MM-dd"),
                TimeResa = TimeOnly.ParseExact(reservationDTO.TimeResa, "HH:mm"),
                NumberOfGuest = reservationDTO.NumberOfGuest,
                Comment = reservationDTO.Comment,
                OccupationStatusOnBook = reservationDTO.OccupationStatusOnBook,
                CreatedBy = reservationDTO.CreatedBy,
                FreeTable21 = reservationDTO.FreeTable21
            };

            _contexte.Reservations.Add(reservation);
            await _contexte.SaveChangesAsync();

            return reservation;
        }

        public async Task<ReservationEntity?> UpdateReservation(int id, ReservationDTO reservationDTO)
        {
            // Vérification des champs obligatoires
            if (string.IsNullOrWhiteSpace(reservationDTO.DateResa))
            {
                throw new ArgumentException("La date de réservation est obligatoire.", nameof(reservationDTO.DateResa));
            }
            if (string.IsNullOrWhiteSpace(reservationDTO.TimeResa))
            {
                throw new ArgumentException("L'heure de réservation est obligatoire.", nameof(reservationDTO.DateResa));
            }

            if (reservationDTO.NumberOfGuest <= 0)
            {
                throw new ArgumentException("Le nombre de personnes doit être supérieur à zéro.", nameof(reservationDTO.NumberOfGuest));
            }

            var reservation = await _contexte.Reservations.Include(r => r.Client).FirstOrDefaultAsync(r => r.Id == id);
            if (reservation == null)
            {
                return null;
            }

            // Supprimer les allocations liées à cette réservation
            _allocationService.DeleteAllocations(id);

            // Mise à jour des informations de la réservation
            reservation.DateResa = DateOnly.ParseExact(reservationDTO.DateResa, "yyyy-MM-dd");
            reservation.TimeResa = TimeOnly.ParseExact(reservationDTO.TimeResa, "HH:mm");
            reservation.NumberOfGuest = reservationDTO.NumberOfGuest;
            reservation.Comment = reservationDTO.Comment;
            reservation.OccupationStatusOnBook = reservationDTO.OccupationStatusOnBook;
            reservation.FreeTable21 = reservationDTO.FreeTable21;
            reservation.CreatedBy = reservationDTO.CreatedBy;
            reservation.UpdatedBy = reservationDTO.UpdatedBy;
            reservation.UpdateTimeStamp = DateTime.Now;
            reservation.Placed = "N";

            _contexte.Reservations.Update(reservation);
            await _contexte.SaveChangesAsync();

            return reservation;
        }


        public async Task<ReservationEntity?> ValidateReservation(int id)
        {
            var reservation = await _contexte.Reservations.Include(r => r.Client).FirstOrDefaultAsync(r => r.Id == id);
            if (reservation == null) { return null; }

            reservation.Status = "C";
            _contexte.Reservations.Update(reservation);
            await _contexte.SaveChangesAsync();

            return reservation;
        }

        public async Task<ReservationEntity?> AnnulerReservation(int id, string user)
        {
            var reservation = await _contexte.Reservations.Include(r => r.Client).FirstOrDefaultAsync(r => r.Id == id);
            if (reservation == null) { return null; }

            // Supprimer les allocations liées à cette réservation
            _allocationService.DeleteAllocations(id);

            // Mise à jour du statut de la réservation
            reservation.Status = "A";
            reservation.CanceledBy = user;
            reservation.Placed = "N";
            reservation.CanceledTimeStamp = DateTime.Now;

            _contexte.Reservations.Update(reservation);
            await _contexte.SaveChangesAsync();

            return reservation;
        }

        public async Task<ReservationEntity?> RefuserReservation(int id, string user)
        {
            var reservation = await _contexte.Reservations.Include(r => r.Client).FirstOrDefaultAsync(r => r.Id == id);
            if (reservation == null) { return null; }

            // Supprimer les allocations liées à cette réservation
            _allocationService.DeleteAllocations(id);

            // Mise à jour du statut de la réservation
            reservation.Status = "R";
            reservation.CanceledBy = user;
            reservation.Placed = "N";
            reservation.CanceledTimeStamp = DateTime.Now;

            _contexte.Reservations.Update(reservation);
            await _contexte.SaveChangesAsync();

            return reservation;
        }
    }
}
