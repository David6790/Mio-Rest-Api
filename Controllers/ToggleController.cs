using Microsoft.AspNetCore.Mvc;
using Mio_Rest_Api.Services.Mio_Rest_Api.Services;
using System;
using System.Threading.Tasks;

namespace Mio_Rest_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToggleController : ControllerBase
    {
        private readonly IServiceToggle _serviceToggle;

        public ToggleController(IServiceToggle serviceToggle)
        {
            _serviceToggle = serviceToggle;
        }

        /// <summary>
        /// Récupérer l'état du toggle de notification.
        /// </summary>
        /// <returns>Retourne un booléen indiquant si la notification est active ou non.</returns>
        [HttpGet("notification-status")]
        public async Task<IActionResult> GetNotificationStatus()
        {
            try
            {
                // Récupère le nombre de notifications au lieu de l'état booléen
                var notificationCount = await _serviceToggle.GetNotificationCountAsync();

                // Renvoie la valeur du compteur dans la réponse
                return Ok(new { count = notificationCount });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erreur interne : {ex.Message}" });
            }
        }

    }
}
