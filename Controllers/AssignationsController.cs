using Microsoft.AspNetCore.Mvc;
using Mio_Rest_Api.DTO;
using Mio_Rest_Api.Entities;
using Mio_Rest_Api.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mio_Rest_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignationsController : ControllerBase
    {
        private readonly IServiceAssignation _serviceAssignation;

        public AssignationsController(IServiceAssignation serviceAssignation)
        {
            _serviceAssignation = serviceAssignation;
        }

        // POST: api/Assignations
        [HttpPost]
        public async Task<ActionResult<List<Assignation>>> PostAssignations(AssignationRequestDto assignationRequestDto)
        {
            try
            {
                var assignations = await _serviceAssignation.AddAssignations(assignationRequestDto);
                return Ok(assignations);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/Assignations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Assignation>>> GetAssignations()
        {
            var assignations = await _serviceAssignation.GetAssignations();
            return Ok(assignations);
        }

        // GET: api/Assignations/period?date=2024-05-07&period=midi
        [HttpGet("period")]
        public async Task<ActionResult<IEnumerable<Assignation>>> GetAssignationsByPeriod(DateOnly date, string period)
        {
            var assignations = await _serviceAssignation.GetAssignationsByPeriod(date, period);
            return Ok(assignations);
        }

        // GET: api/Assignations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Assignation>> GetAssignation(int id)
        {
            try
            {
                var assignation = await _serviceAssignation.GetAssignation(id);
                return Ok(assignation);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/Assignations/available?date=2024-05-07&period=midi
        [HttpGet("availableTables")]
        public async Task<ActionResult<IEnumerable<TableDto>>> GetAvailableTables(DateOnly date, string period)
        {
            var tables = await _serviceAssignation.GetAvailableTables(date, period);
            return Ok(tables);
        }
    }
}
