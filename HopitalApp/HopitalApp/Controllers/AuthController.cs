using HopitalApp.Data;
using HopitalApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HopitalApp.DTOs;

namespace HopitalApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly HopitalDbContext _context;

        public AuthController(HopitalDbContext context)
        {
            _context = context;
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
                return Unauthorized(new
                {
                    Message = "Login ou mot de passe incorrect"
                });
            }

            return Ok(new
            {
                Message = "Connexion réussie",
                Role = user.Role
            });
        }
    }
}