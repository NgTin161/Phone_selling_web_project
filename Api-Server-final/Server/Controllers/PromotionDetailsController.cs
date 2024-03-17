using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionDetailsController : ControllerBase
    {
        private readonly PhoneshopIdentityContext _context;
        public PromotionDetailsController(PhoneshopIdentityContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PromotionDetail>>> GetPromotionDetail()
        {
            return await _context.PromotionDetail.Include(p => p.Promotion)
                                                 .Include(p => p.PhoneModel)
                                                 .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PromotionDetail>> GetPromotionDetail(int id)
        {
            var promotionDetail = await _context.PromotionDetail.Include(p => p.Promotion)
                                                 .Include(p => p.PhoneModel)
                                                 .FirstOrDefaultAsync(p => p.Id == id);
            if (promotionDetail == null) { return  NotFound(); }

            return promotionDetail;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPromotionDetal(int id, PromotionDetail promotionDetail)
        {
            if(id != promotionDetail.Id)
            {
                return BadRequest();
            }
            _context.Entry(promotionDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!PromotionDetailExists(id)) { 
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
        public async Task<ActionResult<PromotionDetail>> PostPromotionDetail(PromotionDetail promotionDetail)
        {
            _context.PromotionDetail.Add(promotionDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPromotionDetail", new { id = promotionDetail.Id }, promotionDetail);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePromotionDetail(int id)
        {
            var promotionDetail = await _context.PromotionDetail.FindAsync(id);
            if (promotionDetail == null) { return NotFound(); }

            _context.PromotionDetail.Remove(promotionDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PromotionDetailExists(int id) {
            return _context.PromotionDetail.Any(p => p.Id == id);
        }
    }
}
