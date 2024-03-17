using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RamsController : ControllerBase
    {
        private readonly PhoneshopIdentityContext _context;

        public RamsController(PhoneshopIdentityContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ram>>> GetRams()
        {
            return await _context.Ram.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Ram>> GetRam(int id)
        {
            var ram = await _context.Ram.FindAsync(id);

            if (ram == null)
            {
                return NotFound();
            }

            return ram;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRam(int id, Ram ram)
        {
            if (id != ram.Id)
            {
                return BadRequest();
            }

            _context.Entry(ram).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RamExists(id))
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
        public async Task<ActionResult<Ram>> PostRam(Ram ram)
        {
            _context.Ram.Add(ram);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRam", new { id = ram.Id }, ram);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRam(int id)
        {
            var ram = await _context.Ram.FindAsync(id);
            if (ram == null)
            {
                return NotFound();
            }

            ram.Status = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RamExists(int id)
        {
            return _context.Ram.Any(e => e.Id == id);
        }
    }
}
