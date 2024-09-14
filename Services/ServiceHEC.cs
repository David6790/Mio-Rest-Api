using Microsoft.EntityFrameworkCore;
using Mio_Rest_Api.Data;
using Mio_Rest_Api.DTO;
using Mio_Rest_Api.Entities;
using Mio_Rest_Api.Services.Mio_Rest_Api.Services;
using System.Threading.Tasks;

namespace Mio_Rest_Api.Services
{
    namespace Mio_Rest_Api.Services
    {
        public interface IServiceHEC
        {
            Task<HECStatut> AddStatutAsync(HECStatutDTO statutDTO);
            
        }
    }
    public class ServiceHEC : IServiceHEC
    {
        private readonly ContextApplication _contexte;

        public ServiceHEC(ContextApplication contexte)
        {
            _contexte = contexte;
        }

        #region AddStatutAsync
        public async Task<HECStatut> AddStatutAsync(HECStatutDTO statutDTO)
        {
            // Vérification des champs obligatoires
            if (string.IsNullOrWhiteSpace(statutDTO.Statut))
            {
                throw new ArgumentException("Le statut ne peut pas être vide.");
            }

            // Validation de la réservation liée
            var reservation = await _contexte.Reservations.FirstOrDefaultAsync(r => r.Id == statutDTO.ReservationId);
            if (reservation == null)
            {
                throw new ArgumentException("La réservation spécifiée n'existe pas.");
            }

            // Conversion du timestamp en DateTime
            DateTime createdAt = DateTime.ParseExact(statutDTO.CreatedAt, "yyyy-MM-ddTHH:mm:ss", null);

            // Création du nouvel objet HECStatut
            HECStatut newStatut = new HECStatut
            {
                ReservationId = statutDTO.ReservationId,
                Statut = statutDTO.Statut,
                CreatedAt = createdAt,
                CreatedBy = statutDTO.CreatedBy
            };

            // Ajout et sauvegarde dans la base de données
            _contexte.HECStatuts.Add(newStatut);
            await _contexte.SaveChangesAsync();

            return newStatut;
        }
        #endregion
    }
}
