using HopitalApp.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly HopitalDbContext _context;

    public DashboardController(HopitalDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetStats()
    {
        return Ok(new
        {
            nombrePatients = _context.Patients.Count(),
            nombreMedecins = _context.Medecins.Count(),
            nombreSpecialites = _context.Specialites.Count(),
            nombreRendezVous = _context.RendezVous.Count()
        });
    }
}