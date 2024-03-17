using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using Server.Models.Momo;
using Server.Services;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly PhoneshopIdentityContext _context;
        private IMomoService _momoService;

        private string _payUrl;

        public PaymentMethodsController(PhoneshopIdentityContext context,IMomoService momoService)
        {
            _context = context;
            _momoService = momoService;
        }

        // GET: api/PaymentMethods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentMethod>>> GetPaymentMethod()
        {
            return await _context.PaymentMethod.ToListAsync();
        }

        // GET: api/PaymentMethods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentMethod>> GetPaymentMethod(int id)
        {
            var paymentMethod = await _context.PaymentMethod.FindAsync(id);

            if (paymentMethod == null)
            {
                return NotFound();
            }

            return paymentMethod;
        }

        // PUT: api/PaymentMethods/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaymentMethod(int id, PaymentMethod paymentMethod)
        {
            if (id != paymentMethod.Id)
            {
                return BadRequest();
            }

            _context.Entry(paymentMethod).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentMethodExists(id))
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

        // POST: api/PaymentMethods
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PaymentMethod>> PostPaymentMethod(PaymentMethod paymentMethod)
        {
            _context.PaymentMethod.Add(paymentMethod);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPaymentMethod", new { id = paymentMethod.Id }, paymentMethod);
        }

        // DELETE: api/PaymentMethods/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaymentMethod(int id)
        {
            var paymentMethod = await _context.PaymentMethod.FindAsync(id);
            if (paymentMethod == null)
            {
                return NotFound();
            }

            _context.PaymentMethod.Remove(paymentMethod);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PaymentMethodExists(int id)
        {
            return _context.PaymentMethod.Any(e => e.Id == id);
        }


        [HttpPost]
        [Route("MoMo")]
        public async Task<IActionResult> CreatePaymentUrl([FromQuery] string fullName, [FromQuery] double amount)
        {
            try
            {
                var response = await _momoService.CreatePaymentAsync(fullName, amount);
                var jsonResponse = new { url = response.PayUrl };
                return Ok(jsonResponse);
            }
            catch (Exception ex)
            { 
                return StatusCode(500, new { ErrorMessage = "An error occurred while processing the request." });
            }
        }


        [HttpGet]
        [Route("ConfirmPayment")]
        public IActionResult PaymentCallBack([FromQuery] MomoExecuteResponseModel responseMoMo, string id)
        {
            string errorCode = responseMoMo.errorCode;
            string orderId = responseMoMo.orderId;
            string amount = responseMoMo.amount;
            string date = responseMoMo.responseTime;

            return Redirect($"http://localhost:3000/paymentconfirm?errorcode={Uri.EscapeDataString(errorCode)}&orderid={Uri.EscapeDataString(orderId)}&amount={Uri.EscapeDataString(amount)}&date={Uri.EscapeDataString(date)}");
        }
      
    }
}
