using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
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


    }
    public class ServiceReservations : IServiceReservation
    {
        private readonly ContextApplication _contexte;

        //Constructeur de la classe pour instancier  le db_context
        public ServiceReservations(ContextApplication contexte)
        {
            _contexte = contexte;
        }


        public async Task<List<ReservationEntity>> GetAllReservations()
        {
            return await _contexte.Reservations.Include(r => r.Client).ToListAsync();
        }

        public async Task<List<ReservationEntity>> GetFuturReservations()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            return await _contexte.Reservations.Include(r => r.Client).Where(r => r.DateResa >= today).OrderByDescending(r=> r.CreaTimeStamp).ToListAsync();
        }

        public async Task<ReservationEntity?> GetReservation(int id)
        {
            return await _contexte.Reservations
                .Include(r => r.Client)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<ReservationEntity>> GetReservationsByDate(string date)
        {
            var reservations = await _contexte.Reservations
                .Include(r => r.Client)
                .Where(r => r.DateResa == DateOnly.ParseExact(date, "yyyy-MM-dd"))
                .OrderByDescending(r => r.CreaTimeStamp)
                .ToListAsync();

            return reservations;
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



            if (reservationDTO.OccupationStatusOnBook == "FreeTable21" || reservationDTO.OccupationStatusOnBook == "Service2Complet")
            {
                reservationDTO.FreeTable21 = "Clients prévénus";
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
            var reservation = await _contexte.Reservations.Include(r => r.Client).FirstOrDefaultAsync(r => r.Id == id);
            ;
            if (reservation == null)
            {
                return null;
            }

            reservation.DateResa = DateOnly.ParseExact(reservationDTO.DateResa, "yyyy-MM-dd");
            reservation.TimeResa = TimeOnly.ParseExact(reservationDTO.TimeResa, "HH:mm");
            reservation.NumberOfGuest = reservationDTO.NumberOfGuest;
            reservation.Comment = reservationDTO.Comment;
            reservation.OccupationStatusOnBook = reservationDTO.OccupationStatusOnBook;
            reservation.FreeTable21 = reservationDTO.FreeTable21;
            reservation.CreatedBy = reservationDTO.CreatedBy;
            reservation.UpdatedBy = reservationDTO.UpdatedBy;
            reservation.UpdateTimeStamp = DateTime.Now;

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

            if (reservation == null) { return null; };

            reservation.Status = "A";
            reservation.CanceledBy = user;
            reservation.Placed = "N";
            reservation.CanceledTimeStamp = DateTime.Now;

            _contexte.Reservations.Update(reservation);

            await _contexte.SaveChangesAsync();
            return reservation;
        }

    }
}