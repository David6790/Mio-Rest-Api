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
            catch (Exception ex)
            {
                // Log l'exception si nécessaire et retourne une réponse appropriée
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //// PUT: api/Reservations/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutReservation(int id, Reservation reservation)
        //{
        //    if (id != reservation.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(reservation).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ReservationExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/Reservations
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Reservation>> PostReservation(Reservation reservation)
        //{
        //    _context.Reservations.Add(reservation);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetReservation", new { id = reservation.Id }, reservation);
        //}

        //// DELETE: api/Reservations/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteReservation(int id)
        //{
        //    var reservation = await _context.Reservations.FindAsync(id);
        //    if (reservation == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Reservations.Remove(reservation);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool ReservationExists(int id)
        //{
        //    return _context.Reservations.Any(e => e.Id == id);
        //}
    }
}
