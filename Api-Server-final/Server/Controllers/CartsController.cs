using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg;
using Server.Data;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly PhoneshopIdentityContext _context;
        private readonly UserManager<User> _userManager;
        public CartsController(PhoneshopIdentityContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

      

        [HttpGet]
        [Authorize(Roles = "User")]
        [Route("getitem")]
        public async Task<ActionResult<Cart>> GetCart()
        {
            var username = User.Identity.Name;
            var user = _userManager.FindByNameAsync(username).Result;
            if (user == null)
            {
                return NotFound();
            }
            var cart = await _context.Carts
       .Where(p => p.UserId == user.Id)
       .Select(p => new
       {
           p.PhoneModel,
           Color = p.PhoneModel.Color,
           Storage = p.PhoneModel.Storage,
           Name = p.PhoneModel.Phone.Name,
           Quantity = p.Quantity
       })
       .ToListAsync();

            return Ok(cart);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        [Route("additem")]
        public async Task<ActionResult<Cart>> PostCart([FromBody] Cart cart)
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            var existingCart = _context.Carts
                .FirstOrDefault(c => c.UserId == user.Id && c.PhoneModelId == cart.PhoneModelId);

            if (existingCart != null)
            {
              
                existingCart.Quantity += cart.Quantity;
                await _context.SaveChangesAsync();

                return Ok(existingCart);
            }

            var newcart = new Cart
            {
                UserId = user.Id,
                PhoneModelId = cart.PhoneModelId,
                Quantity = cart.Quantity,
            };

            _context.Carts.Add(newcart);
            await _context.SaveChangesAsync();

            return Ok(newcart);
        }


        [HttpPost]
        [Authorize(Roles = "User")]
        [Route("updateitem")]
        public async Task<ActionResult<Cart>> UpdateItem([FromQuery] int PhoneModelId, int Quantity)
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            var existingCart = _context.Carts
                .FirstOrDefault(c => c.UserId == user.Id && c.PhoneModelId == PhoneModelId);

            if (existingCart != null)
            {
                existingCart.Quantity = Quantity;

                try
                {
                    _context.Update(existingCart);
                    await _context.SaveChangesAsync();
                    return Ok(existingCart);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(existingCart.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return NotFound();
        }


        [HttpPost()]
        [Authorize(Roles = "User")]
        [Route("deleteitem")]
        public async Task<IActionResult> DeleteItem([FromQuery] int PhoneModelId)
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            var existingCart = _context.Carts
                .FirstOrDefault(c => c.UserId == user.Id && c.PhoneModelId == PhoneModelId);

            if (existingCart != null)
            {
                _context.Remove(existingCart);
                await _context.SaveChangesAsync();
                return Ok("Xóa Item thành công");
            }

            // Return NotFound if the cart item is not found
            return NotFound("Item not found in the cart.");
        }

        [HttpDelete()]
        [Authorize(Roles = "User")]
        [Route("deleteallitem")]
        public async Task<IActionResult> DeleteAllItem()
        {
            var username = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            var cartItems = _context.Carts
                .Where(c => c.UserId == user.Id)
                .ToList();

            if (cartItems.Any())
            {
                _context.RemoveRange(cartItems);
                await _context.SaveChangesAsync();
                return Ok("Xóa tất cả sản phẩm thành công");
            }

            // Return NotFound if no items are found in the cart
            return NotFound("No items found in the cart.");
        }



        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.Id == id);
        }
    }
}
