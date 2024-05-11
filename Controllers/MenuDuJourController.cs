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
    public class MenuDuJourController : ControllerBase
    {
        private readonly IServiceMenuDuJour _serviceMenuDuJour;

        public MenuDuJourController(IServiceMenuDuJour serviceMenuDujour)
        {
            _serviceMenuDuJour = serviceMenuDujour;
        }

        // GET: api/MenuDuJour
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuEntity>>> GetMenus()
        {
            try
            {
                var menus = await _serviceMenuDuJour.GetMenus();
                return Ok(menus);
            }
            catch (Exception ex)
            {
                // Log the exception here if you have a logging framework
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //    // GET: api/MenuDuJour/5
        //    [HttpGet("{id}")]
        //    public async Task<ActionResult<MenuEntity>> GetMenuEntity(int id)
        //    {
        //        var menuEntity = await _context.MenuDuJour.FindAsync(id);

        //        if (menuEntity == null)
        //        {
        //            return NotFound();
        //        }

        //        return menuEntity;
        //    }

        //    // PUT: api/MenuDuJour/5
        //    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //    [HttpPut("{id}")]
        //    public async Task<IActionResult> PutMenuEntity(int id, MenuEntity menuEntity)
        //    {
        //        if (id != menuEntity.Id)
        //        {
        //            return BadRequest();
        //        }

        //        _context.Entry(menuEntity).State = EntityState.Modified;

        //        try
        //        {
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!MenuEntityExists(id))
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

        //    // POST: api/MenuDuJour
        //    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //    [HttpPost]
        //    public async Task<ActionResult<MenuEntity>> PostMenuEntity(MenuEntity menuEntity)
        //    {
        //        _context.MenuDuJour.Add(menuEntity);
        //        await _context.SaveChangesAsync();

        //        return CreatedAtAction("GetMenuEntity", new { id = menuEntity.Id }, menuEntity);
        //    }

        //    // DELETE: api/MenuDuJour/5
        //    [HttpDelete("{id}")]
        //    public async Task<IActionResult> DeleteMenuEntity(int id)
        //    {
        //        var menuEntity = await _context.MenuDuJour.FindAsync(id);
        //        if (menuEntity == null)
        //        {
        //            return NotFound();
        //        }

        //        _context.MenuDuJour.Remove(menuEntity);
        //        await _context.SaveChangesAsync();

        //        return NoContent();
        //    }

        //    private bool MenuEntityExists(int id)
        //    {
        //        return _context.MenuDuJour.Any(e => e.Id == id);
        //    }
        }
    }
