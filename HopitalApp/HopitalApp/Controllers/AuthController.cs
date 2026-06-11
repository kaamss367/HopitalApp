using HopitalApp.Data;
using HopitalApp.DTOs;
using HopitalApp.Models;
using HopitalApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HopitalApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly HopitalDbContext _context;
        private readonly LogService       _logService;
        private readonly PasswordService  _passwordService;

        public AuthController(HopitalDbContext context, LogService logService, PasswordService passwordService)
        {
            _context         = context;
            _logService      = logService;
            _passwordService = passwordService;
        }

        // ── Connexion ──────────────────────────────────────────────────────────
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Login == dto.Login);

            if (user == null)
            {
                _logService.EcrireLog($"Connexion refusée (login inconnu) : {dto.Login}");
                return Unauthorized("Identifiants incorrects");
            }

            bool motDePasseValide;

            if (_passwordService.EstDejaHache(user.MotDePasse))
            {
                // Mot de passe déjà haché → vérification BCrypt normale
                motDePasseValide = _passwordService.Verifier(dto.MotDePasse, user.MotDePasse);
            }
            else
            {
                // Mot de passe encore en clair → comparaison directe puis migration au vol
                motDePasseValide = user.MotDePasse == dto.MotDePasse;

                if (motDePasseValide)
                {
                    user.MotDePasse = _passwordService.Hacher(dto.MotDePasse);
                    await _context.SaveChangesAsync();
                    _logService.EcrireLog($"Migration automatique du mot de passe : {user.Login}");
                }
            }

            if (!motDePasseValide)
            {
                _logService.EcrireLog($"Connexion refusée (mauvais mdp) : {dto.Login}");
                return Unauthorized("Identifiants incorrects");
            }

            _logService.EcrireLog($"Connexion réussie : {user.Login}");
            return Ok(new { message = "Connexion réussie", role = user.Role });
        }

        // ── Créer un utilisateur (mot de passe haché automatiquement) ──────────
        [HttpPost("CreerUtilisateur")]
        public async Task<IActionResult> CreerUtilisateur([FromBody] CreerUtilisateurDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Login) || string.IsNullOrWhiteSpace(dto.MotDePasse))
                return BadRequest("Login et mot de passe obligatoires.");

            if (dto.MotDePasse.Length < 6)
                return BadRequest("Le mot de passe doit contenir au moins 6 caractères.");

            var existe = await _context.Utilisateurs.AnyAsync(u => u.Login == dto.Login);
            if (existe)
                return Conflict("Ce login existe déjà.");

            var utilisateur = new Utilisateur
            {
                Login      = dto.Login,
                MotDePasse = _passwordService.Hacher(dto.MotDePasse),
                Role       = dto.Role
            };

            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();

            _logService.EcrireLog($"Nouvel utilisateur créé : {dto.Login} (rôle : {dto.Role})");
            return Ok(new { message = "Utilisateur créé avec succès." });
        }

        // ── Migration : hache tous les mots de passe encore en clair ──────────
        // À appeler UNE SEULE FOIS après déploiement.
        [HttpPost("MigreMotsDePasse")]
        public async Task<IActionResult> MigreMotsDePasse()
        {
            var utilisateurs = await _context.Utilisateurs.ToListAsync();
            int count = 0;

            foreach (var u in utilisateurs)
            {
                if (!_passwordService.EstDejaHache(u.MotDePasse))
                {
                    u.MotDePasse = _passwordService.Hacher(u.MotDePasse);
                    count++;
                }
            }

            await _context.SaveChangesAsync();
            _logService.EcrireLog($"Migration mots de passe : {count} compte(s) mis à jour.");
            return Ok(new { message = $"{count} mot(s) de passe migré(s) avec succès." });
        }
    }
}
