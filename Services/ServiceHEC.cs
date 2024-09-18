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
            Task<List<HECStatut>> GetStatutsByReservationIdAsync(int reservationId); 
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

            
            DateTime createdAt = DateTime.Now;

            // Création du nouvel objet HECStatut
            HECStatut newStatut = new HECStatut
            {
                ReservationId = statutDTO.ReservationId,
                Actions = statutDTO.Actions,
                Statut = statutDTO.Statut,
                Libelle = statutDTO.Libelle,
                CreatedAt = createdAt,
                CreatedBy = statutDTO.CreatedBy
            };

            // Ajout et sauvegarde dans la base de données
            _contexte.HECStatuts.Add(newStatut);
            await _contexte.SaveChangesAsync();

            return newStatut;
        }
        #endregion

        #region GetStatutsByReservationIdAsync
        public async Task<List<HECStatut>> GetStatutsByReservationIdAsync(int reservationId)
        {
            try
            {
                // Validation de la réservation
                var reservationExists = await _contexte.Reservations.AnyAsync(r => r.Id == reservationId);
                if (!reservationExists)
                {
                    throw new ArgumentException("La réservation spécifiée n'existe pas.");
                }

                // Récupérer tous les statuts liés à la réservation et les trier par CreatedAt
                var statuts = await _contexte.HECStatuts
                    .Where(s => s.ReservationId == reservationId)
                    .OrderBy(s => s.CreatedAt) // Tri par CreatedAt (ordre croissant)
                    .ToListAsync();

                // Vérification si aucun statut n'a été trouvé
                if (statuts == null || statuts.Count == 0)
                {
                    throw new InvalidOperationException("Aucun statut trouvé pour la réservation spécifiée.");
                }

                return statuts;
            }
            catch (ArgumentException ex)
            {
                // Gestion des erreurs liées aux arguments invalides (ex: réservation inexistante)
                throw new ArgumentException($"Erreur lors de la récupération des statuts : {ex.Message}");
            }
            catch (Exception ex)
            {
                // Gestion des erreurs générales
                throw new Exception($"Erreur interne lors de la récupération des statuts : {ex.Message}");
            }
        }
        #endregion

    }
}
