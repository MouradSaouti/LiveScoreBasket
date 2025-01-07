using LiveScore.Server.Data;
using LiveScore.Server.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LiveScore.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CinqDeBaseController : ControllerBase
    {
        private readonly BasketDbContext _context;

        public CinqDeBaseController(BasketDbContext context)
        {
            _context = context;
        }

        [HttpGet("{matchId}")]
        public IActionResult GetCinqDeBaseByMatchId(int matchId)
        {
            var cinqDeBase = _context.CinqsDeBase.Where(c => c.MatchId == matchId).ToList();
            if (!cinqDeBase.Any())
            {
                return NotFound("Aucun cinq de base configuré pour ce match.");
            }
            return Ok(cinqDeBase);
        }

        [HttpPost]
        public IActionResult AddPlayerToCinqDeBase(int equipeId, int joueurId, int matchId)
        {
            // Vérifiez que le match existe
            var match = _context.Matchs.FirstOrDefault(m => m.Id == matchId);
            if (match == null)
            {
                return NotFound("Match introuvable.");
            }

            // Vérifiez que l'équipe existe
            var equipe = _context.Equipes.FirstOrDefault(e => e.Id == equipeId);
            if (equipe == null)
            {
                return NotFound("Équipe introuvable.");
            }

            // Vérifiez que le joueur existe et appartient à l'équipe
            var joueur = _context.Joueurs.FirstOrDefault(j => j.Id == joueurId && j.EquipeId == equipeId);
            if (joueur == null)
            {
                return NotFound("Joueur introuvable ou n'appartient pas à l'équipe spécifiée.");
            }

            // Vérifiez que le joueur n'est pas déjà dans le cinq de base
            var existingCinq = _context.CinqsDeBase.FirstOrDefault(c => c.MatchId == matchId && c.EquipeId == equipeId && c.JoueurId == joueurId);
            if (existingCinq != null)
            {
                return BadRequest("Ce joueur est déjà dans le cinq de base.");
            }

            // Vérifiez qu'il n'y a pas plus de 5 joueurs dans le cinq de base
            var totalPlayersInCinq = _context.CinqsDeBase.Count(c => c.MatchId == matchId && c.EquipeId == equipeId);
            if (totalPlayersInCinq >= 5)
            {
                return BadRequest("L'équipe a déjà 5 joueurs dans le cinq de base.");
            }

            // Ajoutez le joueur au cinq de base
            var cinqDeBase = new CinqDeBase
            {
                MatchId = matchId,
                EquipeId = equipeId,
                JoueurId = joueurId,
            };

            _context.CinqsDeBase.Add(cinqDeBase);
            _context.SaveChanges();

            return Ok(new { Message = "Joueur ajouté au cinq de base avec succès." });
        }
    }
}
