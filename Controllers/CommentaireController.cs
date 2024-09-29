using Microsoft.AspNetCore.Mvc;
using Mio_Rest_Api.DTO;
using Mio_Rest_Api.Services;
using System;
using System.Threading.Tasks;

namespace Mio_Rest_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentaireController : ControllerBase
    {
        private readonly IServiceCommentaire _serviceCommentaire;

        public CommentaireController(IServiceCommentaire serviceCommentaire)
        {
            _serviceCommentaire = serviceCommentaire;
        }

        /// <summary>
        /// Ajouter un nouveau commentaire pour une réservation.
        /// </summary>
        /// <param name="commentaireDTO">Le DTO contenant les informations du commentaire à ajouter.</param>
        /// <param name="origin">Optionnel : Identifiant de l'origine (ex: "Client" ou "Restaurant")</param>
        /// <returns>Retourne le commentaire ajouté ou un message d'erreur en cas d'échec.</returns>
        [HttpPost("add-commentaire")]
        public async Task<IActionResult> AddCommentaire([FromBody] CommentaireDTO commentaireDTO, [FromQuery] string? origin = null)
        {
            try
            {
                var newCommentaire = await _serviceCommentaire.AddCommentaireAsync(commentaireDTO, origin);
                return CreatedAtAction(nameof(GetCommentairesByReservationId), new { reservationId = newCommentaire.ReservationId }, newCommentaire);
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
        /// Récupérer tous les commentaires liés à une réservation donnée, triés par date de création.
        /// </summary>
        /// <param name="reservationId">L'ID de la réservation.</param>
        /// <returns>Retourne la liste des commentaires ou un message d'erreur en cas d'échec.</returns>
        [HttpGet("commentaires/{reservationId}")]
        public async Task<IActionResult> GetCommentairesByReservationId(int reservationId)
        {
            try
            {
                var commentaires = await _serviceCommentaire.GetCommentairesByReservationIdAsync(reservationId);
                return Ok(commentaires);
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
