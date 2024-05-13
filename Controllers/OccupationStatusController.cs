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
    public class OccupationStatusController : ControllerBase
    {
        private readonly IServiceOccupation _serviceOccupationStatus;

        public OccupationStatusController(IServiceOccupation service)
        {
            _serviceOccupationStatus = service;
        }

        // GET: api/OccupationStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OccupationStatus>>> GetAllOccupationStatus()
        {
            try
            {
                var occupations = await _serviceOccupationStatus.GetAllOccupationStatus();
                return Ok(occupations);
            }
            catch (Exception ex)
            {
                // Log the exception here if you have a logging framework
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOccupationStatus( OccupationDTO occupationDTO)
        {
            try
            {
                if (occupationDTO == null)
                {
                    return BadRequest("Occupation data must be provided");
                }

                var (occupations, conflict) = await _serviceOccupationStatus.AddOccupationStatus(occupationDTO);
                if (conflict)
                {
                    // Si un conflit est détecté, retourner la liste des occupations existantes avec un message approprié
                    return Conflict(new { Message = "Occupation status for the date already exists.", Occupations = occupations });
                }

                // Sinon, retourner la nouvelle occupation créée
                return CreatedAtAction(nameof(GetAllOccupationStatus), new { date = occupationDTO.DateOfEffect }, occupations[0]);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
       [HttpGet("ByDate/{date}")]
public async Task<ActionResult<OccupationStatusDetailDTO>> GetOccupationStatusByDate(string date)
{
    try
    {
        var dateOfEffect = DateOnly.ParseExact(date, "yyyy-MM-dd");
        var occupationStatusDetail = await _serviceOccupationStatus.GetOccupationStatusByDate(dateOfEffect);

        if (occupationStatusDetail == null)
        {
            return NotFound($"No OccupationStatus found for date: {date}");
        }

        return Ok(occupationStatusDetail);
    }
    catch (FormatException)
    {
        return BadRequest("Invalid date format. Please use yyyy-MM-dd format.");
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}




        //    // GET: api/OccupationStatus/5
        //    [HttpGet("{id}")]
        //    public async Task<ActionResult<OccupationStatus>> GetOccupationStatus(int id)
        //    {
        //        var occupationStatus = await _context.OccupationStatus.FindAsync(id);

        //        if (occupationStatus == null)
        //        {
        //            return NotFound();
        //        }

        //        return occupationStatus;
        //    }

        //    // PUT: api/OccupationStatus/5
        //    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //    [HttpPut("{id}")]
        //    public async Task<IActionResult> PutOccupationStatus(int id, OccupationStatus occupationStatus)
        //    {
        //        if (id != occupationStatus.Id)
        //        {
        //            return BadRequest();
        //        }

        //        _context.Entry(occupationStatus).State = EntityState.Modified;

        //        try
        //        {
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!OccupationStatusExists(id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }

        //        return NoContent();
        //    }

        //    // POST: api/OccupationStatus
        //    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //    [HttpPost]
        //    public async Task<ActionResult<OccupationStatus>> PostOccupationStatus(OccupationStatus occupationStatus)
        //    {
        //        _context.OccupationStatus.Add(occupationStatus);
        //        await _context.SaveChangesAsync();

        //        return CreatedAtAction("GetOccupationStatus", new { id = occupationStatus.Id }, occupationStatus);
        //    }

        //    // DELETE: api/OccupationStatus/5
        //    [HttpDelete("{id}")]
        //    public async Task<IActionResult> DeleteOccupationStatus(int id)
        //    {
        //        var occupationStatus = await _context.OccupationStatus.FindAsync(id);
        //        if (occupationStatus == null)
        //        {
        //            return NotFound();
        //        }

        //        _context.OccupationStatus.Remove(occupationStatus);
        //        await _context.SaveChangesAsync();

        //        return NoContent();
        //    }

        //    private bool OccupationStatusExists(int id)
        //    {
        //        return _context.OccupationStatus.Any(e => e.Id == id);
        //    }
    }
}
