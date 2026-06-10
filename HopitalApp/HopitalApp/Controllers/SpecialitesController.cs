using HopitalApp.Data;
using HopitalApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HopitalApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecialitesController : ControllerBase
    {
        private readonly HopitalDbContext _context;

        public SpecialitesController(HopitalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Specialite>>> GetSpecialites()
        {
            return await _context.Specialites.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Specialite>> GetSpecialite(int id)
        {
            var specialite = await _context.Specialites.FindAsync(id);

            if (specialite == null)
                return NotFound();

            return specialite;
        }

        [HttpPost]
        public async Task<ActionResult<Specialite>> PostSpecialite(Specialite specialite)
        {
            _context.Specialites.Add(specialite);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSpecialite), new { id = specialite.Id }, specialite);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpecialite(int id, Specialite specialite)
        {
            if (id != specialite.Id)
                return BadRequest();

            _context.Entry(specialite).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpecialite(int id)
        {
            var specialite = await _context.Specialites.FindAsync(id);

            if (specialite == null)
                return NotFound();

            _context.Specialites.Remove(specialite);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}