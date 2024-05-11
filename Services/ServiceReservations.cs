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
        Task<ReservationEntity?> GetReservation(int id);
        Task<ReservationEntity> CreateReservation(ReservationDTO reservationDTO);

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
            return await _contexte.Reservations.Include(r=>r.Client).ToListAsync();
        }

        public async Task<ReservationEntity?> GetReservation(int id)
        {
            return await _contexte.Reservations
                .Include(r => r.Client)  
                .FirstOrDefaultAsync(r => r.Id == id);  
        }

        public async Task<ReservationEntity>CreateReservation(ReservationDTO reservationDTO)
        {
            Client? client = await _contexte.Clients.FirstOrDefaultAsync(c =>
            
                c.Name == reservationDTO.ClientName && c.Prenom == reservationDTO.ClientPrenom
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

            ReservationEntity reservation = new ReservationEntity
            {
                IdClient = client.Id,
                DateResa = DateOnly.ParseExact(reservationDTO.DateResa, "yyyy-MM-dd"),
                TimeResa = TimeOnly.ParseExact(reservationDTO.TimeResa, "HH:mm"),
                NumberOfGuest = reservationDTO.NumberOfGuest,
                Comment = reservationDTO.Comment,
            };

            _contexte.Reservations.Add(reservation);
            await _contexte.SaveChangesAsync();

            return reservation;
        }

    }
}
