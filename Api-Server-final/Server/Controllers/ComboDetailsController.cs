using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComboDetailsController : ControllerBase
    {
        private readonly PhoneshopIdentityContext _context;
        public ComboDetailsController(PhoneshopIdentityContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComboDetail>>> GetComboDetail() 
        {
            return await _context.ComboDetails.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ComboDetail>> GetComboDetail(int id)
        {
            var comboDetail = await _context.ComboDetails.FindAsync(id);
            if(comboDetail == null) { return NotFound(); }

            return comboDetail;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutComboDetail(int id, ComboDetail comboDetail)
        {
             if(id != comboDetail.Id)
            {
                return BadRequest();
            }
             _context.Entry(comboDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComboDetailExists(id))
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
        public async Task<ActionResult<ComboDetail>> PostComboDetail(ComboDetail comboDetail)
        {
            _context.ComboDetails.Add(comboDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComboDetail", new {id =  comboDetail.Id}, comboDetail);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComboDetail(int id)
        {
            var comboDetail = await _context.ComboDetails.FindAsync(id);
            if (comboDetail == null) { return NotFound(); }

            _context.ComboDetails.Remove(comboDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ComboDetailExists(int id)
        {
            return _context.ComboDetails.Any(x => x.Id == id);
        }
    }
}
