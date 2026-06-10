using HopitalApp.Data;
using HopitalApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HopitalApp.DTOs;
using HopitalApp.Services;

namespace HopitalApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly HopitalDbContext _context;
        private readonly LogService _logService;

        public AuthController(HopitalDbContext context, LogService logService)
        {
            _context = context;
            _logService = logService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto LoginDto)
        {
            var user = await _context.Utilisateurs
                .FirstOrDefaultAsync(u =>
                    u.Login == LoginDto.Login &&
                    u.MotDePasse == LoginDto.MotDePasse);

            if (user == null)
            {
                _logService.EcrireLog($"Connexion refusée : {LoginDto.Login}");
                return Unauthorized("Identifiants incorrects");
            }

            _logService.EcrireLog($"Connexion réussie : {user.Login}");

            return Ok(new
            {
                message = "Connexion réussie",
                role = user.Role
            });

        }
    }
}