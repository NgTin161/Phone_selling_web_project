using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorsController : ControllerBase
    {
        private readonly PhoneshopIdentityContext _context;

        public ColorsController(PhoneshopIdentityContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Color>>> GetColors()
        {
            return Ok(await _context.Colors.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Color>> GetColor(int id)
        {
            var color = await _context.Colors.FindAsync(id);

            if (color == null)
            {
                return NotFound();
            }

            return color;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutColor(int id, Color color)
        {
            if (id != color.Id)
            {
                return BadRequest();
            }

            _context.Entry(color).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ColorExists(id))
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
        public async Task<ActionResult<Color>> PostColor(Color color)
        {
            _context.Colors.Add(color);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetColor", new { id = color.Id }, color);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColor(int id)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null)
            {
                return NotFound();
            }

            color.Status = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ColorExists(int id)
        {
            return _context.Colors.Any(e => e.Id == id);
        }
    }
}
