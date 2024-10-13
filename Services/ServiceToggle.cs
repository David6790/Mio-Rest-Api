using Microsoft.EntityFrameworkCore;
using Mio_Rest_Api.Data;
using System.Threading.Tasks;

namespace Mio_Rest_Api.Services.Mio_Rest_Api.Services
{
    public interface IServiceToggle
    {
        Task<int> GetNotificationCountAsync(); // Récupérer le nombre de notifications
        Task IncrementNotificationCountAsync(); // Incrémenter le compteur
        Task DecrementNotificationCountAsync(); // Décrémenter le compteur

        Task<int> GetCommentNotificationCountAsync(); // Récupérer le nombre de notifications pour les commentaires
        Task IncrementCommentNotificationCountAsync(); // Incrémenter le compteur pour les commentaires
        Task DecrementCommentNotificationCountAsync(); // Décrémenter le compteur pour les commentaires
    }

    public class ServiceToggle : IServiceToggle
    {
        private readonly ContextApplication _context;

        public ServiceToggle(ContextApplication context)
        {
            _context = context;
        }

        #region GetNotificationCountAsync
        public async Task<int> GetNotificationCountAsync()
        {
            // Récupérer les deux lignes en une seule requête SQL
            var toggles = await _context.Toggles
                .Where(t => t.Name == "Notification" || t.Name == "Commentaire")
                .ToListAsync();

            // Récupérer les valeurs de NotificationCount et s'assurer que les lignes existent
            var notificationCount = toggles.FirstOrDefault(t => t.Name == "Notification")?.NotificationCount ?? 0;
            var commentaireCount = toggles.FirstOrDefault(t => t.Name == "Commentaire")?.NotificationCount ?? 0;

            // Retourner la somme des deux valeurs
            return notificationCount + commentaireCount;
        }

        #endregion

        #region IncrementNotificationCountAsync
        public async Task IncrementNotificationCountAsync()
        {
            var toggle = await _context.Toggles.FirstOrDefaultAsync(t => t.Name == "Notification");
            if (toggle == null)
            {
                throw new ArgumentException("Le toggle 'Notification' n'existe pas.");
            }

            toggle.NotificationCount += 1;
            toggle.LastUpdated = DateTime.Now;

            _context.Toggles.Update(toggle);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region DecrementNotificationCountAsync
        public async Task DecrementNotificationCountAsync()
        {
            var toggle = await _context.Toggles.FirstOrDefaultAsync(t => t.Name == "Notification");
            if (toggle == null)
            {
                throw new ArgumentException("Le toggle 'Notification' n'existe pas.");
            }

            if (toggle.NotificationCount > 0)
            {
                toggle.NotificationCount -= 1;
            }
            toggle.LastUpdated = DateTime.Now;

            _context.Toggles.Update(toggle);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region GetCommentNotificationCountAsync
        public async Task<int> GetCommentNotificationCountAsync()
        {
            var toggle = await _context.Toggles.FirstOrDefaultAsync(t => t.Name == "Commentaire");
            if (toggle == null)
            {
                throw new ArgumentException("Le toggle 'Commentaire' n'existe pas.");
            }

            return toggle.NotificationCount; // On suppose que ce champ gère les notifications des commentaires
        }
        #endregion

        #region IncrementCommentNotificationCountAsync
        public async Task IncrementCommentNotificationCountAsync()
        {
            var toggle = await _context.Toggles.FirstOrDefaultAsync(t => t.Name == "Commentaire");
            if (toggle == null)
            {
                throw new ArgumentException("Le toggle 'Commentaire' n'existe pas.");
            }

            toggle.NotificationCount += 1;
            toggle.LastUpdated = DateTime.Now;

            _context.Toggles.Update(toggle);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region DecrementCommentNotificationCountAsync
        public async Task DecrementCommentNotificationCountAsync()
        {
            var toggle = await _context.Toggles.FirstOrDefaultAsync(t => t.Name == "Commentaire");
            if (toggle == null)
            {
                throw new ArgumentException("Le toggle 'Commentaire' n'existe pas.");
            }

            if (toggle.NotificationCount > 0)
            {
                toggle.NotificationCount -= 1;
            }
            toggle.LastUpdated = DateTime.Now;

            _context.Toggles.Update(toggle);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}
