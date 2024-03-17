using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoragesController : ControllerBase
    {
        private readonly PhoneshopIdentityContext _context;

        public StoragesController(PhoneshopIdentityContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Storage>>> GetStorages()
        {
            return await _context.Storage.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Storage>> GetStorage(int id)
        {
            var storage = await _context.Storage.FindAsync(id);

            if (storage == null)
            {
                return NotFound();
            }

            return storage;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutStorage(int id, Storage storage)
        {
            if (id != storage.Id)
            {
                return BadRequest();
            }

            _context.Entry(storage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StorageExists(id))
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
        public async Task<ActionResult<Storage>> PostStorage(Storage storage)
        {
            _context.Storage.Add(storage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStorage", new { id = storage.Id }, storage);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStorage(int id)
        {
            var storage = await _context.Storage.FindAsync(id);
            if (storage == null)
            {
                return NotFound();
            }

            storage.Status = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StorageExists(int id)
        {
            return _context.Storage.Any(e => e.Id == id);
        }
    }
}
