using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mio_Rest_Api.Data;
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
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            var reservations = await _serviceReservations.GetReservations();
            return Ok(reservations);
        }

        // GET: api/Reservations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var reservation = await _serviceReservations.GetReservation(id);

            if (reservation == null)
            {
                return NotFound();
            }

            return Ok(reservation);
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
