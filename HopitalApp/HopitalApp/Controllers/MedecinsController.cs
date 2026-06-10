using HopitalApp.Data;
using HopitalApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HopitalApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedecinsController : ControllerBase
    {
        private readonly HopitalDbContext _context;

        public MedecinsController(HopitalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Medecin>>> GetMedecins()
        {
            return await _context.Medecins
                .Include(m => m.Specialite)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Medecin>> GetMedecin(int id)
        {
            var medecin = await _context.Medecins
                .Include(m => m.Specialite)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medecin == null)
                return NotFound();

            return medecin;
        }

        [HttpPost]
        public async Task<ActionResult<Medecin>> PostMedecin(Medecin medecin)
        {
            var specialiteExiste = await _context.Specialites
                .AnyAsync(s => s.Id == medecin.SpecialiteId);

            if (!specialiteExiste)
                return BadRequest("La spécialité indiquée n'existe pas.");

            _context.Medecins.Add(medecin);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMedecin), new { id = medecin.Id }, medecin);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMedecin(int id, Medecin medecin)
        {
            if (id != medecin.Id)
                return BadRequest();

            var specialiteExiste = await _context.Specialites
                .AnyAsync(s => s.Id == medecin.SpecialiteId);

            if (!specialiteExiste)
                return BadRequest("La spécialité indiquée n'existe pas.");

            _context.Entry(medecin).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedecin(int id)
        {
            var medecin = await _context.Medecins.FindAsync(id);

            if (medecin == null)
                return NotFound();

            _context.Medecins.Remove(medecin);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}