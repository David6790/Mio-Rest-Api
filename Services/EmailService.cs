using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using DotNetEnv;

namespace Mio_Rest_Api.Services.Mio_Rest_Api.Services
{
    public interface IEmailService
    {
        Task SendPendingResaAlertAsync(string toEmail, string clientName, int numberOfGuests, DateTime dateTime, int resId);
    }
    public class EmailService : IEmailService
    {
     
        private readonly string _apiKey;

        public EmailService()
        {
            // Charger l'API key depuis le fichier .env
            Env.Load();
            _apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");

            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new Exception("La clé API SendGrid est manquante dans le fichier .env.");
            }
        }

        // Envoi d'un email de confirmation de réservation en attente
        public async Task SendPendingResaAlertAsync(string toEmail, string clientName, int numberOfGuests, DateTime dateTime, int resId)
        {
            string templateId = "d-50b411a9135f4e89a8e2432771968b45"; // Remplace par l'ID réel du template dans SendGrid

            var templateData = new
            {
                clientName = clientName,
                numberOfGuest = numberOfGuests,
                dateTime = dateTime.ToString("dd/MM/yyyy HH:mm"),
                dateTimeEN = dateTime.ToString("MMMM dd, yyyy HH:mm"),
                resId = resId
            };

            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("david@reserver-simplement.fr", "Il Girasole");
            var to = new EmailAddress(toEmail);

            var msg = new SendGridMessage
            {
                From = from,
                TemplateId = templateId
            };

            // Ajouter le destinataire et les données du template
            msg.AddTo(to);
            msg.SetTemplateData(templateData);

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Échec de l'envoi de l'email. Code de statut: {response.StatusCode}");
            }
        }
    }
}
