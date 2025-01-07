using LiveScore.Server.Data;
using LiveScore.Server.Hubs;
using LiveScore.Server.Models;
using LiveScore.Server.DTO;
using LiveScore.Server.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace LiveScore.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EvenementsMatchController : ControllerBase
    {
        private readonly BasketDbContext _context;
        private readonly IHubContext<MatchHub> _hubContext;

        public EvenementsMatchController(BasketDbContext context, IHubContext<MatchHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        [HttpPost("panier")]
        public async Task<IActionResult> EnregistrerPanier([FromBody] EventDto dto)
        {
            if (dto == null || dto.MatchId <= 0 || dto.JoueurId <= 0 || dto.EquipeId <= 0 || dto.Points == null || dto.Points < 1 || dto.Points > 3 || dto.EncodageUserId <= 0)
            {
                return BadRequest("Les données envoyées sont invalides.");
            }

            try
            {
                var panierEvent = new EvenementMatch
                {
                    MatchId = dto.MatchId,
                    JoueurId = dto.JoueurId,
                    TypeEvenement = "Panier",
                    Points = dto.Points,
                    Temps = dto.Temps ?? DateTime.UtcNow.ToString("HH:mm:ss"),
                    Timestamp = DateTime.UtcNow,
                    EquipeId = dto.EquipeId,
                    EncodageUserId = dto.EncodageUserId
                };

                _context.EvenementsMatch.Add(panierEvent);
                await _context.SaveChangesAsync();

                // Diffuser l'événement uniquement au groupe correspondant
                await _hubContext.Clients.All.SendAsync("Receive update:", dto.MatchId ,"Il a marquer" );

                return Ok(new { Message = "Panier enregistré avec succès.", Evenement = panierEvent });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne : {ex.Message}");
            }
        }



        [HttpPost("faute")]
        public async Task<IActionResult> EnregistrerFaute([FromBody] EventDto eventDto)
        {
            if (eventDto.TypeEvenement.ToLower() != "faute" || string.IsNullOrEmpty(eventDto.TypeFaute))
            {
                return BadRequest("Type d'événement ou type de faute invalide.");
            }

            var match = await _context.Matchs.FindAsync(eventDto.MatchId);
            if (match == null)
            {
                return NotFound("Match introuvable.");
            }

            var joueurFautif = await _context.Joueurs.FindAsync(eventDto.JoueurId);
            if (joueurFautif == null)
            {
                return BadRequest("Joueur fautif introuvable.");
            }

            var fauteEvent = new EvenementMatch
            {
                MatchId = eventDto.MatchId,
                JoueurId = eventDto.JoueurId,
                TypeEvenement = "Faute",
                TypeFaute = eventDto.TypeFaute,
                Temps = eventDto.Temps ?? DateTime.UtcNow.ToString("HH:mm:ss"),
                Timestamp = DateTime.UtcNow,
                EquipeId = eventDto.EquipeId,
                EncodageUserId = eventDto.EncodageUserId,
                JoueurVictimeId = eventDto.JoueurVictimeId
            };

            _context.EvenementsMatch.Add(fauteEvent);
            await _context.SaveChangesAsync();

            // Diffusion SignalR
            await _hubContext.Clients.Group($"match_{eventDto.MatchId}")
                .SendAsync("ReceiveEvent", fauteEvent);

            return Ok(new { Message = "Faute enregistrée avec succès.", Evenement = fauteEvent });
        }





        [HttpPost("changement")]
        public async Task<IActionResult> EnregistrerChangement([FromBody] EventDto dto)
        {
            var joueurSortant = await _context.Joueurs.FindAsync(dto.JoueurSortantId);
            var joueurEntrant = await _context.Joueurs.FindAsync(dto.JoueurEntrantId);
            var match = await _context.Matchs.FindAsync(dto.MatchId);

            if (joueurSortant == null || joueurEntrant == null || match == null)
                return NotFound("Joueur sortant, joueur entrant ou match introuvable.");

            if (joueurSortant.EquipeId != joueurEntrant.EquipeId)
                return BadRequest("Les joueurs doivent appartenir à la même équipe pour un changement.");

            if (joueurSortant.EquipeId != match.EquipeDomicileId && joueurSortant.EquipeId != match.EquipeExterieurId)
                return BadRequest("Les joueurs ne sont pas associés à une équipe du match.");

            var equipe = await _context.Equipes.FindAsync(joueurSortant.EquipeId);
            if (equipe == null)
                return BadRequest("L'équipe associée au joueur sortant est introuvable.");

            var evenement = new EvenementMatch
            {
                MatchId = dto.MatchId,
                JoueurId = joueurSortant.Id,
                TypeEvenement = nameof(EventType.Changement),
                JoueurSortantId = joueurSortant.Id,
                JoueurEntrantId = joueurEntrant.Id,
                Timestamp = dto.Timestamp,
                EncodageUserId = dto.EncodageUserId,
                EquipeId = joueurSortant.EquipeId // Assurez-vous que cette valeur est correcte
            };



            _context.EvenementsMatch.Add(evenement);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group($"match_{dto.MatchId}").SendAsync("ReceiveChangement", evenement);

            return Ok(evenement);
        }

        [HttpPost("timeout")]
        public async Task<IActionResult> EnregistrerTimeout([FromBody] EventDto dto)
        {
            var match = await _context.Matchs.FindAsync(dto.MatchId);
            if (match == null)
            {
                return NotFound("Match introuvable.");
            }

            var timeoutEvent = new EvenementMatch
            {
                MatchId = dto.MatchId,
                TypeEvenement = "Timeout",
                EquipeId = dto.EquipeId,
                Temps = dto.Temps ?? DateTime.UtcNow.ToString("HH:mm:ss"),
                Timestamp = DateTime.UtcNow,
                EncodageUserId = dto.EncodageUserId
            };

            _context.EvenementsMatch.Add(timeoutEvent);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Timeout enregistré avec succès.", Evenement = timeoutEvent });
        }


        [HttpGet("{matchId}")]
        public async Task<IActionResult> GetEvenementsByMatchId(int matchId)
        {
            var evenements = await _context.EvenementsMatch
                .Where(e => e.MatchId == matchId)
                .OrderBy(e => e.Timestamp)
                .ToListAsync();

            if (!evenements.Any())
            {
                return NotFound("Aucun événement trouvé pour ce match.");
            }

            return Ok(evenements);
        }


    }
}
