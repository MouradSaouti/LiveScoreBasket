using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LiveScore.Server.Data;
using LiveScore.Server.Models;
using LiveScore.Server.DTO;
using Microsoft.AspNetCore.Authorization;

namespace LiveScore.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipesController : ControllerBase
    {
        private readonly BasketDbContext _context;

        public EquipesController(BasketDbContext context)
        {
            _context = context;
        }

        // GET: api/Equipes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipeDto>>> GetEquipes()
        {
            var equipes = await _context.Equipes
                .Include(e => e.Joueurs)
                .ToListAsync();

            var equipesDto = equipes.Select(e => new EquipeDto
            {
                Id = e.Id,
                Nom = e.Nom,
                Code = e.Code,
                EstEquipeDomicile = e.EstEquipeDomicile,
                Joueurs = e.Joueurs.Select(j => new JoueurDto
                {
                    Id = j.Id,
                    Nom = j.Nom,
                    Numero = j.Numero,
                    Position = j.Position,
                    EquipeId = e.Id
                }).ToList()
            }).ToList();

            return Ok(equipesDto);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<EquipeDto>> GetEquipeById(int id)
        {
            var equipe = await _context.Equipes.Include(e => e.Joueurs).FirstOrDefaultAsync(e => e.Id == id);

            if (equipe == null)
            {
                return NotFound();
            }

            var equipeDto = new EquipeDto
            {
                Id = equipe.Id,
                Nom = equipe.Nom,
                Code = equipe.Code,
                EstEquipeDomicile = equipe.EstEquipeDomicile,
                Joueurs = equipe.Joueurs.Select(j => new JoueurDto
                {
                    Id = j.Id,
                    Nom = j.Nom,
                    Prenom = j.Prenom,
                    Taille = j.Taille,
                    estCapitaine = j.estCapitaine,
                    estEnJeu = j.estEnJeu,
                    Numero = j.Numero,
                    Position = j.Position,
                    EquipeId = j.EquipeId
                }).ToList()
            };

            return Ok(equipeDto);
        }




        // POST: api/Equipes
        [HttpPost]
        public async Task<ActionResult<EquipeDto>> PostEquipe(EquipeDto equipeDto)
        {
            var equipe = new Equipe
            {
                Nom = equipeDto.Nom,
                Code = equipeDto.Code,
                EstEquipeDomicile = equipeDto.EstEquipeDomicile,
            };

            _context.Equipes.Add(equipe);
            await _context.SaveChangesAsync();

            equipeDto.Id = equipe.Id;
            return CreatedAtAction(nameof(GetEquipes), new { id = equipe.Id }, equipeDto);
        }

        // DELETE: api/Equipes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipe(int id)
        {
            var equipe = await _context.Equipes.FindAsync(id);
            if (equipe == null)
            {
                return NotFound();
            }

            _context.Equipes.Remove(equipe);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
