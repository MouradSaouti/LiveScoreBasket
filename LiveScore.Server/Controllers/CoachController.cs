using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LiveScore.Server.Data;
using LiveScore.Server.Models;
using LiveScore.Server.DTO;

namespace LiveScore.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoachController : ControllerBase
    {
        private readonly BasketDbContext _context;

        public CoachController(BasketDbContext context)
        {
            _context = context;
        }

        // GET: api/Coach
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoachDto>>> GetCoachs()
        {
            var coachs = await _context.Coachs.Include(c => c.Equipe).ToListAsync();

            var coachsDto = coachs.Select(c => new CoachDto
            {
                Id = c.Id,
                Nom = c.Nom,
                EquipeId = c.EquipeId
            }).ToList();

            return Ok(coachsDto);
        }

        // GET: api/Coach/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CoachDto>> GetCoachById(int id)
        {
            var coach = await _context.Coachs.Include(c => c.Equipe).FirstOrDefaultAsync(c => c.Id == id);

            if (coach == null)
            {
                return NotFound();
            }

            var coachDto = new CoachDto
            {
                Id = coach.Id,
                Nom = coach.Nom,
                EquipeId = coach.EquipeId
            };

            return Ok(coachDto);
        }

        [HttpPost]
        public async Task<ActionResult<CoachDto>> AddCoach(CoachDto coachDto)
        {
            try
            {
                if (string.IsNullOrEmpty(coachDto.Nom))
                {
                    return BadRequest("Nom is required.");
                }

                var coach = new Coach
                {
                    Nom = coachDto.Nom,
                    EquipeId = coachDto.EquipeId
                };

                _context.Coachs.Add(coach);
                await _context.SaveChangesAsync();

                coachDto.Id = coach.Id;
                return CreatedAtAction(nameof(GetCoachById), new { id = coach.Id }, coachDto);
            }
            catch (Exception ex)
            {
                // Log the error to your logging system
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
        }


        // DELETE: api/Coach/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoach(int id)
        {
            var coach = await _context.Coachs.FindAsync(id);
            if (coach == null)
            {
                return NotFound();
            }

            _context.Coachs.Remove(coach);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
