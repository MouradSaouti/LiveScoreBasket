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
    public class JoueurController : ControllerBase
    {
        private readonly BasketDbContext _context;

        public JoueurController(BasketDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JoueurDto>>> GetJoueurs()
        {
            var joueurs = await _context.Joueurs.Include(j => j.Equipe).ToListAsync();

            var joueursDto = joueurs.Select(j => new JoueurDto
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
            }).ToList();

            return Ok(joueursDto);
        }


        // GET: api/Joueur/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JoueurDto>> GetJoueurById(int id)
        {
            var joueur = await _context.Joueurs.Include(j => j.Equipe).FirstOrDefaultAsync(j => j.Id == id);

            if (joueur == null)
            {
                return NotFound();
            }

            var joueurDto = new JoueurDto
            {
                Id = joueur.Id,
                Nom = joueur.Nom,
                Prenom = joueur.Prenom,
                Taille = joueur.Taille,
                estCapitaine = joueur.estCapitaine,
                estEnJeu = joueur.estEnJeu,
                Numero = joueur.Numero,
                Position = joueur.Position,
                EquipeId = joueur.EquipeId
            };

            return Ok(joueurDto);
        }

        [HttpPost]
        public async Task<ActionResult<JoueurDto>> AddJoueur(JoueurDto joueurDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Vérifiez si l'équipe existe
            var equipe = await _context.Equipes.FindAsync(joueurDto.EquipeId);
            if (equipe == null)
            {
                return NotFound($"L'équipe avec l'ID {joueurDto.EquipeId} n'existe pas.");
            }

            var joueur = new Joueur
            {
                Nom = joueurDto.Nom,
                Prenom = joueurDto.Prenom,
                Taille = joueurDto.Taille,
                estCapitaine = joueurDto.estCapitaine,
                estEnJeu = joueurDto.estEnJeu,
                Numero = joueurDto.Numero,
                Position = joueurDto.Position,
                EquipeId = joueurDto.EquipeId
            };

            _context.Joueurs.Add(joueur);
            await _context.SaveChangesAsync();

            joueurDto.Id = joueur.Id;
            joueurDto.EquipeNom = equipe.Nom; // Ajoutez le nom de l'équipe

            return CreatedAtAction(nameof(GetJoueurById), new { id = joueur.Id }, joueurDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJoueur(int id, JoueurDto joueurDto)
        {
            if (id != joueurDto.Id)
            {
                return BadRequest("L'ID du joueur ne correspond pas.");
            }

            var joueur = await _context.Joueurs.FindAsync(id);
            if (joueur == null)
            {
                return NotFound();
            }

            // Vérifiez si l'équipe existe
            var equipe = await _context.Equipes.FindAsync(joueurDto.EquipeId);
            if (equipe == null)
            {
                return NotFound($"L'équipe avec l'ID {joueurDto.EquipeId} n'existe pas.");
            }

            joueur.Nom = joueurDto.Nom;
            joueur.Prenom = joueurDto.Prenom;
            joueur.Taille = joueurDto.Taille;
            joueur.estCapitaine = joueurDto.estCapitaine;
            joueur.estEnJeu = joueurDto.estEnJeu;
            joueur.Numero = joueurDto.Numero;
            joueur.Position = joueurDto.Position;
            joueur.EquipeId = joueurDto.EquipeId;

            _context.Entry(joueur).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Joueurs.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // DELETE: api/Joueur/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJoueur(int id)
        {
            var joueur = await _context.Joueurs.FindAsync(id);
            if (joueur == null)
            {
                return NotFound();
            }

            _context.Joueurs.Remove(joueur);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JoueurExists(int id)
        {
            return _context.Joueurs.Any(e => e.Id == id);
        }
    }
}