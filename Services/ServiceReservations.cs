using Microsoft.EntityFrameworkCore;
using Mio_Rest_Api.Data;
using Mio_Rest_Api.Entities;

namespace Mio_Rest_Api.Services
{
    public interface IServiceReservation
    {
        Task<List<Reservation>> GetReservations();
        Task<Reservation> GetReservation(int id);

    }
    public class ServiceReservations : IServiceReservation
    {
        private readonly ContextReservation _contexte;
        public ServiceReservations(ContextReservation contexte)
        {
            _contexte = contexte;
        }
        public async Task<List<Reservation>> GetReservations()
        {
            return await _contexte.Reservations.ToListAsync();
        }

        public async Task<Reservation?> GetReservation(int id)
        {
            return await _contexte.Reservations.FindAsync(id);
        }
    }
}
