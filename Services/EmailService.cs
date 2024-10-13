using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using DotNetEnv;

namespace Mio_Rest_Api.Services.Mio_Rest_Api.Services
{
    public interface IEmailService
    {
        // Méthodes pour les notifications destinées aux clients
        Task SendPendingResaClientAsync(string toEmail, string clientName, int numberOfGuests, DateTime dateTime, int resId);
        Task SendClientMessageRecuAsync(string toEmail, string clientName, int numberOfGuests, DateTime dateTime, int resId);
        Task SendClientModificationConfirmeAsync(string toEmail, string clientName, int numberOfGuests, DateTime dateTime, int resId);
        Task SendClientModificationEnAttenteAsync(string toEmail, string clientName, int numberOfGuests, DateTime dateTime, int resId);
        Task SendClientReservationConfirmeAsync(string toEmail, string clientName, int numberOfGuests, DateTime dateTime, int resId);

        // Méthodes pour les notifications destinées aux gestionnaires
        Task SendNotificationGestionnaireCommentaireAsync(string clientName, string clientComment, DateTime dateTime, int resId);
        Task SendNotificationGestionnaireModificationAsync(string clientName, int numberOfGuests, DateTime dateTime, int resId);
        Task SendNotificationGestionnaireReservationAsync(string clientName, int numberOfGuests, DateTime dateTime, int resId);
    }

    public class EmailService : IEmailService
    {
        private readonly string _apiKey;
        private readonly string _managerEmail;
        private readonly string _ccEmail1;
        private readonly string _ccEmail2;

        public EmailService()
        {
            Env.Load();
            _apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            _managerEmail = Environment.GetEnvironmentVariable("MANAGER_EMAIL");
            _ccEmail1 = Environment.GetEnvironmentVariable("CC_EMAIL_1");
            _ccEmail2 = Environment.GetEnvironmentVariable("CC_EMAIL_2");

            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new Exception("La clé API SendGrid est manquante dans le fichier .env.");
            }
        }

        // Méthode générique pour les emails des clients
        private async Task SendEmailToClientAsync(string toEmail, string templateId, object templateData)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("david@reserver-simplement.fr", "Il Girasole");
            var to = new EmailAddress(toEmail);

            var msg = new SendGridMessage
            {
                From = from,
                TemplateId = templateId
            };

            msg.AddTo(to);
            msg.SetTemplateData(templateData);

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Échec de l'envoi de l'email. Code de statut: {response.StatusCode}");
            }
        }

        // Méthode générique pour les emails aux gestionnaires
        private async Task SendEmailToManagerAsync(string templateId, object templateData)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("david@reserver-simplement.fr", "Il Girasole");
            var to = new EmailAddress(_managerEmail);

            var msg = new SendGridMessage
            {
                From = from,
                TemplateId = templateId
            };

            msg.AddTo(to);

            if (!string.IsNullOrWhiteSpace(_ccEmail1))
            {
                msg.AddCc(new EmailAddress(_ccEmail1));
            }

            if (!string.IsNullOrWhiteSpace(_ccEmail2))
            {
                msg.AddCc(new EmailAddress(_ccEmail2));
            }

            msg.SetTemplateData(templateData);

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Échec de l'envoi de l'email. Code de statut: {response.StatusCode}");
            }
        }

        #region Méthodes pour les clients

        public async Task SendPendingResaClientAsync(string toEmail, string clientName, int numberOfGuests, DateTime dateTime, int resId)
        {
            var templateData = new
            {
                clientName,
                numberOfGuest = numberOfGuests,
                dateTime = dateTime.ToString("dd/MM/yyyy HH:mm"),
                dateTimeEN = dateTime.ToString("MMMM dd, yyyy HH:mm"),
                resId // Ajout de l'ID pour construire le lien
            };
            await SendEmailToClientAsync(toEmail, "d-50b411a9135f4e89a8e2432771968b45", templateData);
        }

        public async Task SendClientMessageRecuAsync(string toEmail, string clientName, int numberOfGuests, DateTime dateTime, int resId)
        {
            var templateData = new
            {
                clientName,
                numberOfGuest = numberOfGuests,
                dateTime = dateTime.ToString("dd/MM/yyyy HH:mm"),
                resId // Ajout de l'ID pour construire le lien
            };
            await SendEmailToClientAsync(toEmail, "d-64f1317e15854920a49064f5b3f6a27d", templateData);
        }

        public async Task SendClientModificationConfirmeAsync(string toEmail, string clientName, int numberOfGuests, DateTime dateTime, int resId)
        {
            var templateData = new
            {
                clientName,
                numberOfGuest = numberOfGuests,
                dateTime = dateTime.ToString("dd/MM/yyyy HH:mm"),
                resId // Ajout de l'ID pour construire le lien
            };
            await SendEmailToClientAsync(toEmail, "d-8a814906f08e48c08c61eebb0978263f", templateData);
        }

        public async Task SendClientModificationEnAttenteAsync(string toEmail, string clientName, int numberOfGuests, DateTime dateTime, int resId)
        {
            var templateData = new
            {
                clientName,
                numberOfGuest = numberOfGuests,
                dateTime = dateTime.ToString("dd/MM/yyyy HH:mm"),
                resId // Ajout de l'ID pour construire le lien
            };
            await SendEmailToClientAsync(toEmail, "d-c10d8c8ad99345698dcaa3e1dad46ad2", templateData);
        }

        public async Task SendClientReservationConfirmeAsync(string toEmail, string clientName, int numberOfGuests, DateTime dateTime, int resId)
        {
            var templateData = new
            {
                clientName,
                numberOfGuest = numberOfGuests,
                dateTime = dateTime.ToString("dd/MM/yyyy HH:mm"),
                dateTimeEN = dateTime.ToString("MMMM dd, yyyy HH:mm"),
                resId // Ajout de l'ID pour construire le lien
            };
            await SendEmailToClientAsync(toEmail, "d-bb9fc72c316e4dde9e66a3ce98ba8e8a", templateData);
        }

        #endregion

        #region Méthodes pour les gestionnaires

        public async Task SendNotificationGestionnaireCommentaireAsync(string clientName, string clientComment, DateTime dateTime, int resId)
        {
            var templateData = new
            {
                clientName,
                clientComment,
                dateTime = dateTime.ToString("dd/MM/yyyy HH:mm"),
                resId
            };
            await SendEmailToManagerAsync("d-a6082f5781e74d8ea6c9d592f35e1e96", templateData);
        }

        public async Task SendNotificationGestionnaireModificationAsync(string clientName, int numberOfGuests, DateTime dateTime, int resId)
        {
            var templateData = new
            {
                clientName,
                numberOfGuest = numberOfGuests,
                dateTime = dateTime.ToString("dd/MM/yyyy HH:mm"),
                resId
            };
            await SendEmailToManagerAsync("d-6cc918a122424be3af1b4866f7569ea0", templateData);
        }

        public async Task SendNotificationGestionnaireReservationAsync(string clientName, int numberOfGuests, DateTime dateTime, int resId)
        {
            var templateData = new
            {
                clientName,
                numberOfGuest = numberOfGuests,
                dateTime = dateTime.ToString("dd/MM/yyyy HH:mm"),
                resId
            };
            await SendEmailToManagerAsync("d-557ae2bcd1064689bdf3bc57dd89a36f", templateData);
        }

        #endregion
    }
}
