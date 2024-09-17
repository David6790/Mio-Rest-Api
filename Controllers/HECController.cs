using Microsoft.AspNetCore.Mvc;
using Mio_Rest_Api.DTO;
using Mio_Rest_Api.Entities;
using Mio_Rest_Api.Services.Mio_Rest_Api.Services;
using System;
using System.Threading.Tasks;

namespace Mio_Rest_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HECController : ControllerBase
    {
        private readonly IServiceHEC _serviceHEC;

        public HECController(IServiceHEC serviceHEC)
        {
            _serviceHEC = serviceHEC;
        }

        /// <summary>
        /// Ajouter un nouveau statut pour une réservation.
        /// </summary>
        /// <param name="statutDTO">Le DTO contenant les informations du statut à ajouter.</param>
        /// <returns>Retourne le statut ajouté ou un message d'erreur en cas d'échec.</returns>
        [HttpPost("add-statut")]
        public async Task<IActionResult> AddStatut([FromBody] HECStatutDTO statutDTO)
        {
            try
            {
                var newStatut = await _serviceHEC.AddStatutAsync(statutDTO);
                return CreatedAtAction(nameof(GetStatutsByReservationId), new { reservationId = newStatut.ReservationId }, newStatut);
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

        /// <summary>
        /// Récupérer tous les statuts liés à une réservation donnée, triés par date de création.
        /// </summary>
        /// <param name="reservationId">L'ID de la réservation.</param>
        /// <returns>Retourne la liste des statuts ou un message d'erreur en cas d'échec.</returns>
        [HttpGet("statuts/{reservationId}")]
        public async Task<IActionResult> GetStatutsByReservationId(int reservationId)
        {
            try
            {
                var statuts = await _serviceHEC.GetStatutsByReservationIdAsync(reservationId);
                return Ok(statuts);
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
