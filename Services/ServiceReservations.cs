using Microsoft.EntityFrameworkCore;
using Mio_Rest_Api.Data;
using Mio_Rest_Api.DTO;
using Mio_Rest_Api.Entities;
using Mio_Rest_Api.Services.Mio_Rest_Api.Services;

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
        Task<ReservationEntity?> ValidateDoubleConfirmation(int id);
        Task<ReservationEntity?> UpdateReservationNotification(int id, string newNotification);
        Task<List<ReservationEntity>> GetUntreatedReservation();

    }

    public class ServiceReservations : IServiceReservation
    {
        private readonly ContextApplication _contexte;
        private readonly IAllocationService _allocationService;
        private readonly IServiceHEC _serviceHEC;
        private readonly IEmailService _emailService;

        // Constructeur pour instancier le db_context et le service d'allocation
        public ServiceReservations(ContextApplication contexte, IAllocationService allocationService, IServiceHEC serviceHEC, IEmailService emailService)
        {
            _contexte = contexte;
            _allocationService = allocationService;
            _serviceHEC = serviceHEC;
            _emailService = emailService;
        }

        #region GetAllReservations
        public async Task<List<ReservationEntity>> GetAllReservations()
        {
            return await _contexte.Reservations.Include(r => r.Client).ToListAsync();
        }
        #endregion

        #region GetUntreatedReservation
        public async Task<List<ReservationEntity>> GetUntreatedReservation()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            return await _contexte.Reservations
                .Include(r => r.Client)
                .Where(r => r.Notifications != NotificationLibelles.PasDeNotification && r.DateResa >= today) // Limiter aux réservations futures
                .ToListAsync();
        }
        #endregion


        #region GetFuturReservations
        public async Task<List<ReservationEntity>> GetFuturReservations()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            return await _contexte.Reservations.Include(r => r.Client).Where(r => r.DateResa >= today).OrderByDescending(r => r.CreaTimeStamp).ToListAsync();
        }
        #endregion

        #region GetReservation
        public async Task<ReservationEntity?> GetReservation(int id)
        {
            return await _contexte.Reservations
                .Include(r => r.Client)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
        #endregion

        #region GetReservationsByDate
        public async Task<List<ReservationEntity>> GetReservationsByDate(string date)
        {
            return await _contexte.Reservations
                .Include(r => r.Client)
                .Where(r => r.DateResa == DateOnly.ParseExact(date, "yyyy-MM-dd"))
                .OrderByDescending(r => r.CreaTimeStamp)
                .ToListAsync();
        }
        #endregion

        #region CreateReservation
        public async Task<ReservationEntity> CreateReservation(ReservationDTO reservationDTO)
        {
            // Validation des champs obligatoires, comme déjà fait...

            // Validation de la date et de l'heure, comme déjà fait...

            // Création du client ou mise à jour du nombre de réservations
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

            // Création de la réservation
            ReservationEntity reservation = new ReservationEntity
            {
                IdClient = client.Id,
                DateResa = DateOnly.ParseExact(reservationDTO.DateResa, "yyyy-MM-dd"),
                TimeResa = TimeOnly.ParseExact(reservationDTO.TimeResa, "HH:mm"),
                NumberOfGuest = reservationDTO.NumberOfGuest,
                Comment = reservationDTO.Comment,
                OccupationStatusOnBook = reservationDTO.OccupationStatusOnBook,
                CreatedBy = reservationDTO.CreatedBy,
                FreeTable21 = reservationDTO.FreeTable21,
                Notifications = NotificationLibelles.NouvelleReservation

            };

            _contexte.Reservations.Add(reservation);
            await _contexte.SaveChangesAsync(); // Sauvegarde de la réservation dans la base de données

            // Ajout du statut "en attente de validation" dans HEC
            HECStatutDTO statutDTO = new HECStatutDTO
            {
                ReservationId = reservation.Id,
                Libelle = "En attente de validation",
                Actions = "RAS",
                Statut = "Réservation enregistrée: ",
                CreatedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), // Format de la date pour correspondre au DTO
                CreatedBy = reservationDTO.CreatedBy
            };

            await _serviceHEC.AddStatutAsync(statutDTO); // Ajout du statut

            string fullName = $"{reservationDTO.ClientName} {reservationDTO.ClientPrenom}";

            // Construction du DateTime avec la date et l'heure de réservation
            DateTime reservationDateTime = DateTime.ParseExact($"{reservationDTO.DateResa} {reservationDTO.TimeResa}", "yyyy-MM-dd HH:mm", null);

            // Envoi de l'email de confirmation en attente
            await _emailService.SendPendingResaAlertAsync(reservationDTO.ClientEmail, fullName, reservationDTO.NumberOfGuest, reservationDateTime, reservation.Id);


            return reservation;
        }

        #endregion

        #region UpdateReservation
        public async Task<ReservationEntity?> UpdateReservation(int id, ReservationDTO reservationDTO)
        {
            // Vérification des champs obligatoires
            if (string.IsNullOrWhiteSpace(reservationDTO.ClientName))
            {
                throw new ArgumentException("Le nom ne peut pas être vide.");
            }

            if (string.IsNullOrWhiteSpace(reservationDTO.ClientPrenom))
            {
                throw new ArgumentException("Le prénom ne peut pas être vide.");
            }

            if (string.IsNullOrWhiteSpace(reservationDTO.DateResa))
            {
                throw new ArgumentException("La date de réservation est obligatoire.");
            }

            if (string.IsNullOrWhiteSpace(reservationDTO.TimeResa))
            {
                throw new ArgumentException("L'heure de réservation est obligatoire.");
            }

            if (reservationDTO.NumberOfGuest <= 0)
            {
                throw new ArgumentException("Le nombre de personnes doit être supérieur à zéro.");
            }

            // Validation du numéro de téléphone
            if (string.IsNullOrWhiteSpace(reservationDTO.ClientTelephone) || !IsValidPhoneNumber(reservationDTO.ClientTelephone))
            {
                throw new ArgumentException("Le numéro de téléphone n'est pas valide.");
            }

            // Validation de l'email
            if (!string.IsNullOrWhiteSpace(reservationDTO.ClientEmail) && !IsValidEmail(reservationDTO.ClientEmail))
            {
                throw new ArgumentException("L'adresse email fournie est invalide.");
            }

            // Validation de la date et de l'heure
            DateOnly reservationDate = DateOnly.ParseExact(reservationDTO.DateResa, "yyyy-MM-dd");
            TimeOnly reservationTime = TimeOnly.ParseExact(reservationDTO.TimeResa, "HH:mm");
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            TimeOnly now = TimeOnly.FromDateTime(DateTime.Now);

            if (reservationDate < today)
            {
                throw new ArgumentException("Vous ne pouvez pas réserver pour une date passée.");
            }

            if (reservationDate == today && reservationTime < now)
            {
                throw new ArgumentException("Vous ne pouvez pas réserver pour une heure passée aujourd'hui.");
            }

            // Rechercher la réservation existante
            var reservation = await _contexte.Reservations.Include(r => r.Client).FirstOrDefaultAsync(r => r.Id == id);
            if (reservation == null)
            {
                return null; // ou lever une exception si vous préférez
            }

            // Supprimer les allocations liées à cette réservation
            _allocationService.DeleteAllocations(id);

            // Mise à jour des informations de la réservation

            if ((reservationDTO.OccupationStatusOnBook == "FreeTable21" || reservationDTO.OccupationStatusOnBook == "Service2Complet") && reservationDTO.TimeResa == "19:00")
            {
                reservationDTO.FreeTable21 = "O";
            }

            reservation.DateResa = reservationDate;
            reservation.TimeResa = reservationTime;
            reservation.NumberOfGuest = reservationDTO.NumberOfGuest;
            reservation.Comment = reservationDTO.Comment;
            reservation.FreeTable21 = reservationDTO.FreeTable21;
            reservation.OccupationStatusOnBook = reservationDTO.OccupationStatusOnBook;
            reservation.CreatedBy = reservationDTO.CreatedBy;
            reservation.UpdatedBy = reservationDTO.UpdatedBy;
            reservation.UpdateTimeStamp = DateTime.Now;
            reservation.Placed = "N";
            if (!string.IsNullOrWhiteSpace(reservationDTO.origin))
            {
                reservation.Status = "M";
                reservation.Notifications = NotificationLibelles.ModificationEnAttente;
                
            }
            

            _contexte.Reservations.Update(reservation);
            await _contexte.SaveChangesAsync();

            // Ajout du statut "en attente de validation" dans HEC
            HECStatutDTO statutDTO = new HECStatutDTO
            {
                ReservationId = reservation.Id,
                Libelle = "En attente de validation",
                Actions = "RAS",
                Statut = "Réservation modifiée: ",
                CreatedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), // Format de la date pour correspondre au DTO
                CreatedBy = reservationDTO.CreatedBy
            };

            await _serviceHEC.AddStatutAsync(statutDTO); // Ajout du statut

            return reservation;
        }
        #endregion

        #region ValidateReservation
        public async Task<ReservationEntity?> ValidateReservation(int id)
        {
            var reservation = await _contexte.Reservations.Include(r => r.Client).FirstOrDefaultAsync(r => r.Id == id);
            if (reservation == null) { return null; }

            reservation.Status = "C";
            reservation.Notifications = NotificationLibelles.PasDeNotification;


            _contexte.Reservations.Update(reservation);
            await _contexte.SaveChangesAsync();

            string libelle;
            string statut;

            if (reservation.UpdatedBy == null)
            {
                libelle = "Hâte de vous recevoir";
                statut = "Réservation Validée: ";
            }
            else
            {
                libelle = "A très bientot";
                statut = "Modification Validée: ";
            }

            // Ajout du statut "en attente de validation" dans HEC
            HECStatutDTO statutDTO = new HECStatutDTO
            {
                ReservationId = reservation.Id,
                Libelle = libelle,
                Actions = "RAS",
                Statut = statut,
                CreatedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), // Format de la date pour correspondre au DTO
                CreatedBy = "System"
            };

            await _serviceHEC.AddStatutAsync(statutDTO); // Ajout du statut

            return reservation;
        }
        #endregion

        #region AnnulerReservation
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
            reservation.Notifications = NotificationLibelles.Annulation;

            _contexte.Reservations.Update(reservation);
            await _contexte.SaveChangesAsync();

            // Ajout du statut "en attente de validation" dans HEC
            HECStatutDTO statutDTO = new HECStatutDTO
            {
                ReservationId = reservation.Id,
                Libelle = "",
                Actions = "RAS",
                Statut = "Réservation Annulée: ",
                CreatedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), // Format de la date pour correspondre au DTO
                CreatedBy = "System"
            };

            await _serviceHEC.AddStatutAsync(statutDTO); // Ajout du statut

            return reservation;
        }
        #endregion

        #region RefuserReservation
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

            // Ajout du statut "en attente de validation" dans HEC
            HECStatutDTO statutDTO = new HECStatutDTO
            {
                ReservationId = reservation.Id,
                Libelle = "consultez vos mail pour plus de détails",
                Actions = "RAS",
                Statut = "Réservation refusée: ",
                CreatedAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), // Format de la date pour correspondre au DTO
                CreatedBy = "System"
            };

            await _serviceHEC.AddStatutAsync(statutDTO); // Ajout du statut

            return reservation;
        }
        #endregion

        #region ValidateDoubleConfirmation
        public async Task<ReservationEntity?> ValidateDoubleConfirmation(int id)
        {
            // Rechercher la réservation avec l'ID fourni
            var reservation = await _contexte.Reservations.FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return null; // Retourner null si la réservation n'existe pas
            }

            // Mettre à jour la valeur de DoubleConfirmation
            reservation.DoubleConfirmation = "O";

            // Mettre à jour l'enregistrement dans la base de données
            _contexte.Reservations.Update(reservation);
            await _contexte.SaveChangesAsync();

            // Créer le statut HEC pour la double confirmation
            HECStatutDTO statutDTO = new HECStatutDTO
            {
                ReservationId = reservation.Id,
                Actions = "", // Pas d'action spécifique
                Statut = "Double confirmation reçue", // Statut personnalisé
                Libelle = "Votre réservation est maintenant ferme et définitive", // Message à afficher
                CreatedBy = "SYSTEM" // Ajouté par le système
            };

            // Appel du service pour ajouter le statut HEC
            await _serviceHEC.AddStatutAsync(statutDTO);

            // Retourner la réservation mise à jour
            return reservation;
        }
        #endregion

        #region UpdateReservationNotification
        public async Task<ReservationEntity?> UpdateReservationNotification(int id, string newNotification)
        {
            // Rechercher la réservation existante avec l'ID fourni
            var reservation = await _contexte.Reservations.FirstOrDefaultAsync(r => r.Id == id);

            // Si la réservation n'existe pas, retourner null
            if (reservation == null)
            {
                return null;
            }

            // Mettre à jour uniquement le champ Notifications
            reservation.Notifications = newNotification;

            // Mettre à jour l'enregistrement dans la base de données
            _contexte.Reservations.Update(reservation);
            await _contexte.SaveChangesAsync();

            // Retourner la réservation mise à jour
            return reservation;
        }
        #endregion


        #region Validation Helpers

        // Méthode pour valider le format de l'email
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Méthode pour valider le format du numéro de téléphone
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            // Implémentez la validation de numéro de téléphone selon vos besoins
            return true; // Modifiez cette logique selon les besoins de votre projet
        }

        #endregion
    }
}
