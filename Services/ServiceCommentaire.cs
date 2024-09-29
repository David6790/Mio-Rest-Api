using Microsoft.EntityFrameworkCore;
using Mio_Rest_Api.Data;
using Mio_Rest_Api.DTO;
using Mio_Rest_Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mio_Rest_Api.Services
{
    public interface IServiceCommentaire
    {
        Task<Commentaire> AddCommentaireAsync(CommentaireDTO commentaireDTO, string origin);
        Task<List<Commentaire>> GetCommentairesByReservationIdAsync(int reservationId);
    }

    public class ServiceCommentaire : IServiceCommentaire
    {
        private readonly ContextApplication _contexte;
        private readonly IServiceReservation _serviceReservation;

        public ServiceCommentaire(ContextApplication contexte, IServiceReservation serviceReservation)
        {
            _contexte = contexte;
            _serviceReservation = serviceReservation;
        }

        #region AddCommentaireAsync
        public async Task<Commentaire> AddCommentaireAsync(CommentaireDTO commentaireDTO, string? origin)
        {
            // Vérification des champs obligatoires
            if (string.IsNullOrWhiteSpace(commentaireDTO.Message))
            {
                throw new ArgumentException("Le message du commentaire ne peut pas être vide.");
            }

            // Validation de la réservation liée
            var reservation = await _serviceReservation.GetReservation(commentaireDTO.ReservationId);
            if (reservation == null)
            {
                throw new ArgumentException("La réservation spécifiée n'existe pas.");
            }

            // Création du nouvel objet Commentaire
            var newCommentaire = new Commentaire
            {
                ReservationId = commentaireDTO.ReservationId,
                Message = commentaireDTO.Message,
                Auteur = commentaireDTO.Auteur,
                CreatedAt = DateTime.Now // Générer automatiquement l'heure actuelle côté serveur
            };

            // Ajout et sauvegarde du commentaire dans la base de données
            _contexte.Commentaires.Add(newCommentaire);
            await _contexte.SaveChangesAsync();

            // Déterminer la notification à utiliser en fonction de l'origine
            string notification = string.IsNullOrWhiteSpace(origin) ? NotificationLibelles.NouveauCommentaire : NotificationLibelles.PasDeNotification;

            // Mettre à jour uniquement le champ Notifications de la réservation via la méthode UpdateReservationNotification
            await _serviceReservation.UpdateReservationNotification(reservation.Id, notification);

            return newCommentaire;
        }
        #endregion


        #region GetCommentairesByReservationIdAsync
        public async Task<List<Commentaire>> GetCommentairesByReservationIdAsync(int reservationId)
        {
            try
            {
                // Validation de la réservation
                var reservationExists = await _contexte.Reservations.AnyAsync(r => r.Id == reservationId);
                if (!reservationExists)
                {
                    throw new ArgumentException("La réservation spécifiée n'existe pas.");
                }

                // Récupérer tous les commentaires liés à la réservation, triés par date de création (du plus ancien au plus récent)
                var commentaires = await _contexte.Commentaires
                    .Where(c => c.ReservationId == reservationId)
                    .OrderBy(c => c.CreatedAt)
                    .ToListAsync();

                // Si aucun commentaire n'est trouvé, ajouter un commentaire par défaut
                if (commentaires == null || commentaires.Count == 0)
                {
                    var commentaireParDefaut = new Commentaire
                    {
                        ReservationId = reservationId,
                        Message = "N'hésitez pas à nous écrire pour échanger sur les détails de votre réservation. Nous sommes là pour vous aider à personnaliser votre expérience.",
                        Auteur = "SYSTEM",
                        CreatedAt = DateTime.Now // Date actuelle
                        
                    };

                    // Ajouter ce commentaire par défaut dans la réponse (sans l'enregistrer dans la base de données)
                    commentaires.Add(commentaireParDefaut);
                }

                return commentaires;
            }
            catch (ArgumentException ex)
            {
                // Gestion des erreurs liées aux arguments invalides (ex: réservation inexistante)
                throw new ArgumentException($"Erreur lors de la récupération des commentaires : {ex.Message}");
            }
            catch (Exception ex)
            {
                // Gestion des erreurs générales
                throw new Exception($"Erreur interne lors de la récupération des commentaires : {ex.Message}");
            }
        }


        #endregion
    }
}
