using LiveScore.Server.Data;
using LiveScore.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace LiveScore.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChronoMatchController : ControllerBase
    {
        private readonly BasketDbContext _context;

        public ChronoMatchController(BasketDbContext context)
        {
            _context = context;
        }
        [HttpPost("update-chrono/{matchId}")]
        public IActionResult UpdateChrono(int matchId)
        {
            var match = _context.Matchs
                .Include(m => m.ChronoMatch)
                .FirstOrDefault(m => m.Id == matchId);

            if (match == null)
                return NotFound("Match introuvable.");

            var chrono = match.ChronoMatch;

            if (chrono == null)
                return BadRequest("Chronomètre non trouvé.");

            // Convertir TempsRestant (string) en TimeSpan
            var tempsRestant = TimeSpan.Parse(chrono.TempsRestant); // Convertir la chaîne en TimeSpan

            // Décrémente le temps restant
            tempsRestant = tempsRestant.Subtract(TimeSpan.FromSeconds(1));

            if (tempsRestant.TotalSeconds <= 0)
            {
                chrono.TempsRestant = "00:00:00";
                chrono.Etat = "Stopped";
                match.Statut = "Terminé";
            }
            else
            {
                chrono.TempsRestant = tempsRestant.ToString(@"hh\:mm\:ss"); // Reconvertir en string
            }

            _context.SaveChanges();

            return Ok(new
            {
                TempsRestant = chrono.TempsRestant,
                Etat = chrono.Etat
            });
        }




        [HttpPost("start/{matchId}")]
        public IActionResult StartChrono(int matchId)
        {
            var match = _context.Matchs
                .Include(m => m.ChronoMatch)
                .FirstOrDefault(m => m.Id == matchId);

            if (match == null)
                return NotFound("Match introuvable.");

            var chrono = match.ChronoMatch ?? new ChronoMatch
            {
                MatchId = matchId,
                TempsRestant = "00:12:00", // Par exemple : 12 minutes
                Etat = "Running"
            };

            chrono.Etat = "Running";
            if (chrono.TempsRestant == "00:00:00")
                chrono.TempsRestant = "00:12:00";

            if (match.ChronoMatch == null)
                _context.ChronoMatchs.Add(chrono);

            _context.SaveChanges();

            return Ok(new
            {
                TempsRestant = chrono.TempsRestant,
                Etat = chrono.Etat
            });
        }


        [HttpPost("stop/{matchId}")]
        public IActionResult StopChrono(int matchId)
        {
            try
            {
                var chrono = _context.ChronoMatchs.FirstOrDefault(c => c.MatchId == matchId);

                if (chrono == null)
                    return NotFound(new { message = "Chronomètre introuvable.", matchId });

                chrono.Etat = "Stopped";

                // Convertir TempsRestant de string à TimeSpan
                var tempsRestant = TimeSpan.Parse(chrono.TempsRestant);

                _context.SaveChanges();

                return Ok(new
                {
                    TempsRestant = tempsRestant.TotalSeconds, // TotalSeconds depuis TimeSpan
                    Etat = chrono.Etat
                });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in StopChrono: {ex.Message}");
                return StatusCode(500, new { message = $"Erreur interne : {ex.Message}" });
            }
        }

    }
}

