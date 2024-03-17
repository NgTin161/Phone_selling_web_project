using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CombosController : ControllerBase
    {
        private readonly PhoneshopIdentityContext _context;

        public CombosController(PhoneshopIdentityContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Combo>>> GetCombos()
        {
            return await _context.Combos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Combo>> GetCombo(int id)
        {
            var combo = await _context.Combos.FindAsync(id);

            if (combo == null)
            {
                return NotFound();
            }

            return combo;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCombo(int id, Combo combo)
        {
            if (id != combo.Id)
            {
                return BadRequest();
            }

            _context.Entry(combo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComboExists(id))
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

        [HttpPost]
        public async Task<ActionResult<Combo>> PostCombo(Combo combo)
        {
            _context.Combos.Add(combo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCombo", new { id = combo.Id }, combo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCombo(int id)
        {
            var combo = await _context.Combos.FindAsync(id);
            if (combo == null)
            {
                return NotFound();
            }

            combo.Status = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ComboExists(int id)
        {
            return _context.Combos.Any(e => e.Id == id);
        }
    }
}
