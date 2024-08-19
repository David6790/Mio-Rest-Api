using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mio_Rest_Api.DTO;
using Mio_Rest_Api.Services;

namespace Mio_Rest_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AllocationsController : ControllerBase
    {
        private readonly IAllocationService _allocationService;

        public AllocationsController(IAllocationService allocationService)
        {
            _allocationService = allocationService;
        }

        [HttpPost("create")]
        public IActionResult CreateAllocation([FromBody] CreateAllocationRequestDto request)
        {
            try
            {
                _allocationService.CreateAllocation(request);
                return Ok(new { message = "Allocation créée avec succès" });
            }
            catch (InvalidOperationException ex)
            {
                // Conflict (409) est utilisé pour indiquer qu'il y a un conflit avec l'état actuel de la ressource
                return Conflict(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                // BadRequest (400) est utilisé pour indiquer une requête incorrecte (par exemple, format de date invalide)
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                // InternalServerError (500) pour les erreurs générales non gérées
                return StatusCode(500, new { error = "Une erreur interne est survenue.", details = ex.Message });
            }
        }


        [HttpGet]
        public IActionResult GetAllocations([FromQuery] string date, [FromQuery] string period)
        {
            if (!DateOnly.TryParse(date, out var dateValue))
            {
                return BadRequest("Format de date invalide.");
            }

            try
            {
                var allocations = _allocationService.GetAllocations(dateValue, period);
                return Ok(allocations);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("deleteByReservation/{reservationId}")]
        public IActionResult DeleteAllocationsByReservationId(int reservationId)
        {
            try
            {
                _allocationService.DeleteAllocations(reservationId);
                return Ok(new { message = "Allocations supprimées avec succès, si elles existaient." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

    }
}
