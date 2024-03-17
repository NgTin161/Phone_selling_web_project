using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using System;
using System.Net.WebSockets;
using System.Security.Claims;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly PhoneshopIdentityContext _context;
        private readonly UserManager<User> _userManager;
      
        public InvoicesController(PhoneshopIdentityContext context,UserManager<User> userManager)

        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoice()
        {
            return await _context.Invoices.Include(i => i.User)
                                            .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoice(int id){
            var invoice = await _context.Invoices.Include(i => i.User)
                                        .FirstOrDefaultAsync(i => i.Id == id);

            if(invoice == null)
            {
                return NotFound();
            }

            return invoice;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoice(int id, Invoice invoice)
        {
            if(id != invoice.Id)
            {
                return BadRequest();
            }

            _context.Entry(invoice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!InvoiceExists(id))
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
        public async Task<ActionResult<Invoice>> PostInvoice(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInvoice", new {id = invoice.Id}, invoice);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if(invoice == null) { return NotFound(); }

            invoice.Status = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(x => x.Id == id);
        }


        [HttpPost]
        [Authorize(Roles = "User")]
        [Route("createinvoice")]
        public async Task<IActionResult> CreateInvoice([FromBody] Invoice invoice)
        {
            string name = User.Identity.Name;
            var user = await  _userManager.FindByNameAsync(name);
            try
            {
                string maDonHang = "DH" + DateTime.Now.ToString("yyyyMMddHHmmss");

                var newInvoice = new Invoice
                {
                    Code = maDonHang,
                    IssuedDate = invoice.IssuedDate,
                    ShippingPhone = invoice.ShippingPhone,
                    ShippingAddress = invoice.ShippingAddress,
                    UserId = user.Id,
                    Total = invoice.Total,
                    PaymentMethodId = invoice.PaymentMethodId,
                    StatusPayMent = invoice.StatusPayMent,
                };

                _context.Invoices.Add(newInvoice);
                await _context.SaveChangesAsync();
                int invoiceId = newInvoice.Id;
                await PostInvoiceDetail(invoiceId);
                return Ok(new
                {
                    Status = "Successful"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Status = "Failed",
                    Message = $"An error occurred: {ex.Message}"
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        [Route("postinvoicedetail")]
        public async Task<ActionResult<InvoiceDetail>> PostInvoiceDetail(int invoiceId)
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            var existingCart = _context.Carts
    .Include(c => c.PhoneModel)
    .Where(c => c.UserId == user.Id)
    .ToList();

            if (existingCart.Any())
            {
                var invoiceDetails = new List<InvoiceDetail>();

                foreach (var item in existingCart)
                {
                    var newInvoiceDetail = new InvoiceDetail
                    {
                        InvoiceId = invoiceId,
                        PhoneModelId = item.PhoneModelId,
                        Quantity = item.Quantity,
                        UnitPrice = item.PhoneModel.Price,
                    };

                    invoiceDetails.Add(newInvoiceDetail);
                    _context.InvoicesDetail.Add(newInvoiceDetail);
                }
                _context.Carts.RemoveRange(existingCart);
                await _context.SaveChangesAsync();

                return Ok(invoiceDetails);
            }
            return BadRequest("Cart not found");
        }


    }
}
