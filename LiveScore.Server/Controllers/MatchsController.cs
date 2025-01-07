using LiveScore.Server.Data;
using LiveScore.Server.Hubs;
using LiveScore.Server.Models;
using LiveScore.Server.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace LiveScore.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchsController : ControllerBase
    {
        private readonly IHubContext<MatchHub> _hubContext;
        private readonly BasketDbContext _context;

        public MatchsController(IHubContext<MatchHub> hubContext, BasketDbContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }
 
        [HttpPost]
        public async Task<IActionResult> CreateMatch(MatchDto matchDto)
        {
            var equipeDomicile = await _context.Equipes.FindAsync(matchDto.EquipeDomicileId);
            var equipeExterieur = await _context.Equipes.FindAsync(matchDto.EquipeExterieurId);

            if (equipeDomicile == null || equipeExterieur == null)
            {
                return NotFound("Une ou plusieurs équipes sont introuvables.");
            }

            var match = new Match
            {
                DateMatch = matchDto.DateMatch,
                EquipeDomicileId = matchDto.EquipeDomicileId,
                EquipeExterieurId = matchDto.EquipeExterieurId,
                ScoreDomicile = matchDto.ScoreDomicile,
                ScoreExterieur = matchDto.ScoreExterieur,
                Statut = matchDto.Statut
            };

            _context.Matchs.Add(match);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMatchById), new { id = match.Id }, match);
        }
        [HttpPost("enregistrer-panier")]
        public async Task<IActionResult> EnregistrerPanierEtMettreAJourScore([FromBody] EventDto dto)
        {
            try
            {
                // Vérifier si le match existe
                var match = await _context.Matchs.FindAsync(dto.MatchId);
                if (match == null)
                {
                    return NotFound("Match introuvable.");
                }

                // Mettre à jour le score en fonction de l'équipe
                if (dto.EquipeId == match.EquipeDomicileId)
                {
                    match.ScoreDomicile += dto.Points ?? 0;
                }
                else if (dto.EquipeId == match.EquipeExterieurId)
                {
                    match.ScoreExterieur += dto.Points ?? 0;
                }
                else
                {
                    return BadRequest("L'équipe spécifiée ne correspond pas au match.");
                }

                // Enregistrer l'événement dans la table EvenementsMatch
                var panierEvent = new EvenementMatch
                {
                    MatchId = dto.MatchId,
                    JoueurId = dto.JoueurId,
                    TypeEvenement = dto.TypeEvenement,
                    Points = dto.Points,
                    EquipeId = dto.EquipeId,
                    EncodageUserId = dto.EncodageUserId,
                    Temps = dto.Temps ?? DateTime.UtcNow.ToString("HH:mm:ss")
                };

                _context.EvenementsMatch.Add(panierEvent);
                _context.Matchs.Update(match);

                // Sauvegarder dans la base de données
                await _context.SaveChangesAsync();

                // Envoyer une mise à jour en temps réel via SignalR
                await _hubContext.Clients.All.SendAsync("ScoreUpdated", new
                {
                    MatchId = match.Id,
                    ScoreDomicile = match.ScoreDomicile,
                    ScoreExterieur = match.ScoreExterieur
                });

                return Ok(new
                {
                    Message = "Panier enregistré et score mis à jour avec succès.",
                    Match = match,
                    Event = panierEvent
                });
            }
            catch (DbUpdateException dbEx)
            {
                Console.Error.WriteLine("Erreur lors de la mise à jour de la base de données :", dbEx);
                return StatusCode(500, "Une erreur est survenue lors de l'enregistrement du panier. Vérifiez les données et réessayez.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Erreur générale :", ex);
                return StatusCode(500, "Une erreur inconnue s'est produite.");
            }
        }



        [HttpPut("{id}/statut")]
        public async Task<IActionResult> UpdateStatut(int id, [FromBody] string statut)
        {
            var match = await _context.Matchs.FindAsync(id);

            if (match == null)
            {
                return NotFound("Match introuvable.");
            }

            match.Statut = statut;
            _context.Matchs.Update(match);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("MatchStatutUpdated", match);
            return Ok(match);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetMatchById(int id)
        {
            var match = await _context.Matchs.FindAsync(id);
            if (match == null)
            {
                return NotFound("Match introuvable.");
            }
            return Ok(match);
        }

        [HttpPost("score")] // Update scores in real-time
        public async Task<IActionResult> UpdateScore([FromBody] ScoreUpdateDto dto)
        {
            var match = await _context.Matchs.FindAsync(dto.MatchId);
            if (match == null)
            {
                return NotFound();
            }

            match.ScoreDomicile += dto.ScoreDomicile;
            match.ScoreExterieur += dto.ScoreExterieur;

            _context.Matchs.Update(match);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ScoreUpdated", match);
            return Ok(match);
        }

        [HttpPost("quarter")] // Manage quarters
        public async Task<IActionResult> UpdateQuarter([FromBody] QuarterUpdateDto dto)
        {
            var match = await _context.Matchs.FindAsync(dto.MatchId);
            if (match == null)
            {
                return NotFound();
            }

            match.CurrentQuarter = dto.Quarter;
            _context.Matchs.Update(match);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("QuarterUpdated", match);
            return Ok(match);
        }

        [HttpPost("subplayer")] // Manage player substitutions
        public async Task<IActionResult> SubPlayer([FromBody] SubPlayerDto dto)
        {
            var subPlayer = new SubPlayer
            {
                MatchId = dto.MatchId,
                PlayerOutId = dto.PlayerOutId,
                PlayerInId = dto.PlayerInId,
                Time = dto.Time
            };

            _context.SubPlayers.Add(subPlayer);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("PlayerSubstituted", subPlayer);
            return Ok(subPlayer);
        }

        [HttpPost("timeout")]
        public async Task<IActionResult> Timeout([FromBody] MatchTimeoutDto dto)
        {
            var timeout = new MatchTimeout
            {
                MatchId = dto.MatchId,
                TeamId = dto.TeamId,
                Time = dto.Time
            };

            _context.Timeouts.Add(timeout);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("TimeoutCalled", timeout);
            return Ok(timeout);
        }
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetMatchDetailsWithPlayers(int id)
        {
            var match = await _context.Matchs
                .Include(m => m.EquipeDomicile)
                .ThenInclude(e => e.Joueurs)
                .Include(m => m.EquipeExterieur)
                .ThenInclude(e => e.Joueurs)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (match == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                Match = match,
                EquipeDomicile = match.EquipeDomicile,
                EquipeExterieur = match.EquipeExterieur,
            });
        }
    }
}
        

