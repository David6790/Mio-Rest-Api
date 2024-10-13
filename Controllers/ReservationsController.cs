using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mio_Rest_Api.Data;
using Mio_Rest_Api.DTO;
using Mio_Rest_Api.Entities;
using Mio_Rest_Api.Services;

namespace Mio_Rest_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IServiceReservation _serviceReservations;

        public ReservationsController(IServiceReservation service)
        {
            _serviceReservations = service;
        }

        // GET: api/Reservations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationEntity>>> GetReservations()
        {
            try
            {
                var reservations = await _serviceReservations.GetAllReservations();
                return Ok(reservations);
            }
            catch (Exception ex)
            {
                // Log the exception here if you have a logging framework
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Reservations
        [HttpGet("futur")]
        public async Task<ActionResult<IEnumerable<ReservationEntity>>> GetFuturReservations()
        {
            try
            {
                var reservations = await _serviceReservations.GetFuturReservations();
                return Ok(reservations);
            }
            catch (Exception ex)
            {
                // Log the exception here if you have a logging framework
                return StatusCode(500, $"Internal server error: {ex.Message}");

            }
        }

        // GET: api/Reservations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationEntity>> GetReservation(int id)
        {
            try
            {
                var reservation = await _serviceReservations.GetReservation(id);
                if (reservation == null)
                {
                    return NotFound();
                }
                return Ok(reservation);
            }
            catch (Exception ex)
            {
                // Log the exception here if you have a logging framework
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("byDate/{date}")]
        public async Task<ActionResult<List<ReservationEntity>>> GetReservationsByDate(string date)
        {
            try
            {
                var reservations = await _serviceReservations.GetReservationsByDate(date);
                return Ok(reservations); // Renvoie une liste vide si aucune réservation n'est trouvée
            }
            catch (Exception ex)
            {
                // Log l'exception (vous pouvez utiliser un logger ici)
                Console.WriteLine(ex.Message);

                // Retourne une réponse d'erreur générique
                return StatusCode(500, "Une erreur s'est produite lors de la récupération des réservations.");
            }
        }


        [HttpPost]
        public async Task<ActionResult<ReservationEntity>> CreateReservation(ReservationDTO reservationDTO)
        {
            try
            {
                if (reservationDTO == null)
                {
                    return BadRequest("Reservation data must be provided");
                }

                var reservation = await _serviceReservations.CreateReservation(reservationDTO);
                return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
            }
            catch (ArgumentException ex)
            {
                // Retourne un message d'erreur approprié pour les exceptions liées aux arguments invalides
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                // Log l'exception si nécessaire et retourne une réponse appropriée pour les erreurs internes
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // PUT: api/Reservations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(int id, [FromBody] ReservationDTO reservationDTO)
        {
            if (reservationDTO == null)
            {
                return BadRequest("Reservation data must be provided");
            }

            try
            {
                var updatedReservation = await _serviceReservations.UpdateReservation(id, reservationDTO);
                if (updatedReservation == null)
                {
                    return NotFound("Reservation not found");
                }

                return Ok(updatedReservation);
            }
            catch (ArgumentException ex)
            {
                // Retourne le message d'erreur capturé par le service
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Retourne le message d'erreur capturé par le service
                return Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                // Retourne un message d'erreur générique pour toute autre exception
                return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
            }
        }


        [HttpPatch("{id}/validate")]
        public async Task<IActionResult> PatchReservationStatus(int id)
        {
            try
            {
                var updatedReservation = await _serviceReservations.ValidateReservation(id);
                if (updatedReservation == null)
                {
                    return NotFound();
                }

                return Ok(updatedReservation);
            }
            catch (Exception ex)
            {
                // Log the exception here if you have a logging framework 
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPatch("{id}/cancel/{user}")]
        public async Task<IActionResult> CancelReservationStatus(int id, string user)
        {
            try
            {
                var updatedReservation = await _serviceReservations.AnnulerReservation(id, user);
                if (updatedReservation == null)
                {
                    return NotFound();
                }

                return Ok(updatedReservation);
            }
            catch (Exception ex)
            {
                // Log the exception here if you have a logging framework 
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPatch("{id}/refuse/{user}")]
        public async Task<IActionResult> RefuserReservationStatus(int id, string user)
        {
            try
            {
                var updatedReservation = await _serviceReservations.RefuserReservation(id, user);
                if (updatedReservation == null)
                {
                    return NotFound();
                }

                return Ok(updatedReservation);
            }
            catch (Exception ex)
            {
                // Log the exception here if you have a logging framework 
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("{id}/validateDoubleConfirmation")]
        public async Task<IActionResult> ValidateDoubleConfirmation(int id)
        {
            try
            {
                var updatedReservation = await _serviceReservations.ValidateDoubleConfirmation(id);
                if (updatedReservation == null)
                {
                    return NotFound("Reservation not found");
                }

                return Ok(updatedReservation);
            }
            catch (Exception ex)
            {
                // Log the exception here if you have a logging framework
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("untreated")]
        public async Task<ActionResult<IEnumerable<ReservationEntity>>> GetUntreatedReservations()
        {
            try
            {
                var reservations = await _serviceReservations.GetUntreatedReservation();
                return Ok(reservations);
            }
            catch (Exception ex)
            {
                // Log the exception here if you have a logging framework
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("byDateAndPeriod")]
        public async Task<ActionResult<List<ReservationEntity>>> GetReservationsByDateAndPeriod(string date, string period)
        {
            try
            {
                var reservations = await _serviceReservations.GetReservationsByDateAndPeriod(date, period);
                return Ok(reservations);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"Une erreur s'est produite lors de la récupération des réservations par date et période : {ex.Message}");
            }
        }

        [HttpGet("client-comments")]
        public async Task<ActionResult<IEnumerable<ReservationEntity>>> GetReservationsWithClientComments()
        {
            try
            {
                // Appel de la méthode du service pour obtenir les réservations avec commentaire client
                var reservations = await _serviceReservations.GetReservationsWithClientComments();
                return Ok(reservations);
            }
            catch (Exception ex)
            {
                // Log the exception (si un système de logging est en place)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }





    }
}