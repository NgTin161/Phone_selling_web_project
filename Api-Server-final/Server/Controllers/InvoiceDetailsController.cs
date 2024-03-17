using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceDetailsController : ControllerBase
    {
        private readonly PhoneshopIdentityContext _context;
        private readonly UserManager<User> _userManager;
        public InvoiceDetailsController(PhoneshopIdentityContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvoiceDetail>>> GetInvoiceDetail()
        {
            return await _context.InvoicesDetail.Include(x => x.Invoice).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDetail>> GetInvoiceDetail(int id)
        {
            var invoiceDetail = await _context.InvoicesDetail.Include(i => i.Invoice)
                                                            .FirstOrDefaultAsync();
            if (invoiceDetail == null) {
                return NotFound();
            }

            return invoiceDetail;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoiceDetail(int id, InvoiceDetail invoiceDetail)
        {
            if (id != invoiceDetail.Id)
            {
                return BadRequest();
            }

            _context.Entry(invoiceDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceDetailExists(id))
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

       

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoiceDetail(int id)
        {
            var invoiceDetail = await _context.InvoicesDetail.FindAsync(id);
            if (invoiceDetail == null) { return NotFound(); }

            _context.InvoicesDetail.Remove(invoiceDetail);
            await _context.SaveChangesAsync();
            return NoContent();
        }


    


        private bool InvoiceDetailExists(int id)
        {
            return _context.InvoicesDetail.Any(x => x.Id == id);
        }
    }
}
