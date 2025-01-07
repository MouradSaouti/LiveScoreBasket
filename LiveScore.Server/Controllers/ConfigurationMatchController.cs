using LiveScore.Server.Data;
using LiveScore.Server.Models;
using Microsoft.AspNetCore.Mvc;
using LiveScore.Server.DTO;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace LiveScore.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
     // Applique la restriction au niveau du contrôleur
    public class ConfigurationsMatchController : ControllerBase
    {
        private readonly BasketDbContext _context;

        public ConfigurationsMatchController(BasketDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllConfigurations()
        {
            var configurations = _context.ConfigurationsMatch.ToList();
            return Ok(configurations);
        }

        [HttpGet("{id}")]
        public IActionResult GetConfigurationById(int id)
        {
            var configuration = _context.ConfigurationsMatch
                .Select(c => new ConfigurationDto
                {
                    Id = c.Id,
                    NomMatch = c.NomMatch,
                    DateHeure = c.DateHeure.ToString("yyyy-MM-ddTHH:mm:ss"),
                    NombreQuartTemps = c.NombreQuartTemps,
                    DureeQuartTemps = c.DureeQuartTemps.ToString(),
                    DureeTimeout = c.DureeTimeout.ToString(),
                    EquipeDomicileId = c.EquipeDomicileId,
                    EquipeExterieurId = c.EquipeExterieurId,
                    UserId = c.UserId,
                    EquipeDomicileNom = c.EquipeDomicile.Nom,
                    EquipeExterieurNom = c.EquipeExterieur.Nom
                })
                .FirstOrDefault(c => c.Id == id);

            if (configuration == null)
                return NotFound();

            return Ok(configuration);
        }
     
        [HttpPost]
        public IActionResult CreateConfiguration([FromBody] ConfigurationDto configDto)
        {
            try
            {
                var equipeDomicile = _context.Equipes.FirstOrDefault(e => e.Id == configDto.EquipeDomicileId);
                var equipeExterieur = _context.Equipes.FirstOrDefault(e => e.Id == configDto.EquipeExterieurId);

                if (equipeDomicile == null || equipeExterieur == null)
                {
                    return BadRequest("Une ou plusieurs équipes sont introuvables.");
                }

                var configuration = new ConfigurationMatch
                {
                    NomMatch = configDto.NomMatch,
                    DateHeure = DateTime.Parse(configDto.DateHeure),
                    NombreQuartTemps = configDto.NombreQuartTemps,
                    DureeQuartTemps = TimeSpan.Parse(configDto.DureeQuartTemps),
                    DureeTimeout = TimeSpan.Parse(configDto.DureeTimeout),
                    EquipeDomicileId = configDto.EquipeDomicileId,
                    EquipeExterieurId = configDto.EquipeExterieurId,
                    UserId = configDto.UserId
                };

                _context.ConfigurationsMatch.Add(configuration);
                _context.SaveChanges();

                var match = new Match
                {
                    DateMatch = configuration.DateHeure,
                    EquipeDomicileId = configuration.EquipeDomicileId.Value,
                    EquipeExterieurId = configuration.EquipeExterieurId.Value,
                    ScoreDomicile = 0,
                    ScoreExterieur = 0,
                    Statut = "A Venir",
                    CurrentQuarter = 1
                };

                _context.Matchs.Add(match);
                _context.SaveChanges();

                var result = new
                {
                    Configuration = configDto,
                    MatchId = match.Id
                };

                return CreatedAtAction(nameof(GetConfigurationById), new { id = configuration.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la création : {ex.Message} - {ex.InnerException?.Message}");
            }
        }

        [HttpGet("CheckMatch/{configId}")]
        public IActionResult CheckMatch(int configId)
        {
            var match = _context.Matchs.FirstOrDefault(m => m.Id == configId);

            if (match == null)
            {
                return NotFound("Aucun match associé à cette configuration.");
            }

            return Ok(match);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateConfiguration(
            string nomMatch,
            int id,
            int nombreQuartTemps,
            string dureeQuartTemps,
            string dureeTimeout,
            string dateHeure,
            int userId
        )
        {
            var configuration = _context.ConfigurationsMatch.FirstOrDefault(c => c.Id == id);
            if (configuration == null)
            {
                return NotFound("Configuration introuvable.");
            }

            configuration.NomMatch = nomMatch;
            configuration.NombreQuartTemps = nombreQuartTemps;
            configuration.DureeQuartTemps = TimeSpan.Parse(dureeQuartTemps);
            configuration.DureeTimeout = TimeSpan.Parse(dureeTimeout);
            configuration.DateHeure = DateTime.Parse(dateHeure);
            configuration.UserId = userId;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteConfiguration(int id)
        {
            var configuration = _context.ConfigurationsMatch.FirstOrDefault(c => c.Id == id);
            if (configuration == null)
            {
                return NotFound("Configuration introuvable.");
            }

            _context.ConfigurationsMatch.Remove(configuration);
            _context.SaveChanges();

            return Ok(new { Message = "Configuration supprimée avec succès." });
        }

        [HttpPost("AjouterEquipes/{configId}")]
        public IActionResult AjouterEquipes(int configId, [FromBody] EquipeDto equipesDto)
        {
            var configuration = _context.ConfigurationsMatch.Find(configId);
            if (configuration == null)
            {
                return NotFound($"Configuration avec l'ID {configId} introuvable.");
            }

            configuration.EquipeDomicileId = equipesDto.EquipeDomicileId;
            configuration.EquipeExterieurId = equipesDto.EquipeExterieurId;

            _context.SaveChanges();

            return Ok(configuration);
        }
    }
}
