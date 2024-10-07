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
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOccupationStatus(OccupationDTO occupationDTO)
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
                    return Conflict(new { Message = "Un statut d'occupation est déja configuré pour cette date.", Occupations = occupations });
                }

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
                var errorResponse = new
                {
                    message = "Internal server error.",
                    detail = ex.Message // Détail supplémentaire si nécessaire
                };

                return StatusCode(500, errorResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOccupationStatus(int id)
        {
            try
            {
                var deletedOccupationStatus = await _serviceOccupationStatus.DeleteOccupationStatus(id);
                if (deletedOccupationStatus == null)
                {
                    return NotFound("Occupation status not found.");
                }

                return Ok(deletedOccupationStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateOccupationStatus(int id, [FromBody] UpdateOccupationStatusDTO newOccStatusDTO)
        {
            try
            {
                var updatedOccupationStatus = await _serviceOccupationStatus.UpdateOccupationStatus(
                    id,
                    newOccStatusDTO.OccStatusMidi,
                    newOccStatusDTO.OccStatusDiner
                );

                if (updatedOccupationStatus == null)
                {
                    return NotFound("Occupation status not found.");
                }

                return Ok(updatedOccupationStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
