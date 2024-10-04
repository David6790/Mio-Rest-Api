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
            try
            {
                // Recherche de la ligne avec le nom "Notification"
                var toggle = await _context.Toggles.FirstOrDefaultAsync(t => t.Name == "Notification");

                // Vérifie si l'entrée "Notification" existe
                if (toggle == null)
                {
                    throw new ArgumentException("Le toggle 'Notification' n'existe pas.");
                }

                // Retourne le nombre de notifications
                return toggle.NotificationCount;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"Erreur lors de la récupération du toggle Notification : {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur interne lors de la récupération du toggle Notification : {ex.Message}");
            }
        }
        #endregion

        #region IncrementNotificationCountAsync
        public async Task IncrementNotificationCountAsync()
        {
            try
            {
                // Recherche de la ligne avec le nom "Notification"
                var toggle = await _context.Toggles.FirstOrDefaultAsync(t => t.Name == "Notification");

                // Vérifie si l'entrée "Notification" existe
                if (toggle == null)
                {
                    throw new ArgumentException("Le toggle 'Notification' n'existe pas.");
                }

                // Incrémente le nombre de notifications
                toggle.NotificationCount += 1;
                toggle.LastUpdated = DateTime.Now;

                // Sauvegarde dans la base de données
                _context.Toggles.Update(toggle);
                await _context.SaveChangesAsync();
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"Erreur lors de l'incrémentation du toggle Notification : {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur interne lors de l'incrémentation du toggle Notification : {ex.Message}");
            }
        }
        #endregion

        #region DecrementNotificationCountAsync
        public async Task DecrementNotificationCountAsync()
        {
            try
            {
                // Recherche de la ligne avec le nom "Notification"
                var toggle = await _context.Toggles.FirstOrDefaultAsync(t => t.Name == "Notification");

                // Vérifie si l'entrée "Notification" existe
                if (toggle == null)
                {
                    throw new ArgumentException("Le toggle 'Notification' n'existe pas.");
                }

                // Décrémente le nombre de notifications, en s'assurant qu'il ne passe pas en dessous de zéro
                if (toggle.NotificationCount > 0)
                {
                    toggle.NotificationCount -= 1;
                }
                toggle.LastUpdated = DateTime.Now;

                // Sauvegarde dans la base de données
                _context.Toggles.Update(toggle);
                await _context.SaveChangesAsync();
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"Erreur lors de la décrémentation du toggle Notification : {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur interne lors de la décrémentation du toggle Notification : {ex.Message}");
            }
        }
        #endregion
    }
}
