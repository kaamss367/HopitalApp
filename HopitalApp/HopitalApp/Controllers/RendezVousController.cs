using HopitalApp.Data;
using HopitalApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HopitalApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RendezVousController : ControllerBase
    {
        private readonly HopitalDbContext _context;

        public RendezVousController(HopitalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RendezVous>>> GetRendezVous()
        {
            return await _context.RendezVous
                .Include(r => r.Patient)
                .Include(r => r.Medecin)
                .ThenInclude(m => m.Specialite)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RendezVous>> GetRendezVous(int id)
        {
            var rdv = await _context.RendezVous
                .Include(r => r.Patient)
                .Include(r => r.Medecin)
                .ThenInclude(m => m.Specialite)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rdv == null)
                return NotFound();

            return rdv;
        }

        [HttpPost]
        public async Task<ActionResult<RendezVous>> PostRendezVous(RendezVous rdv)
        {
            if (rdv.DateFin <= rdv.DateDebut)
                return BadRequest("La date de fin doit être supérieure à la date de début.");

            bool conflit = await _context.RendezVous.AnyAsync(r =>
                r.MedecinId == rdv.MedecinId &&
                rdv.DateDebut < r.DateFin &&
                rdv.DateFin > r.DateDebut);

            if (conflit)
                return BadRequest("Conflit d'horaire : le médecin a déjà un rendez-vous sur ce créneau.");

            _context.RendezVous.Add(rdv);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRendezVous), new { id = rdv.Id }, rdv);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRendezVous(int id, RendezVous rdv)
        {
            if (id != rdv.Id)
                return BadRequest();

            bool conflit = await _context.RendezVous.AnyAsync(r =>
                r.Id != id &&
                r.MedecinId == rdv.MedecinId &&
                rdv.DateDebut < r.DateFin &&
                rdv.DateFin > r.DateDebut);

            if (conflit)
                return BadRequest("Conflit d'horaire détecté.");

            _context.Entry(rdv).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRendezVous(int id)
        {
            var rdv = await _context.RendezVous.FindAsync(id);

            if (rdv == null)
                return NotFound();

            _context.RendezVous.Remove(rdv);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}