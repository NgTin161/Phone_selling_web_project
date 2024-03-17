using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using Server.Data;
using Server.Models;
using System;
//D:\DOANHK6\DoAnHK6\Api-Server-final\Api_Server.sln
namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistsController : ControllerBase
    {
        private readonly PhoneshopIdentityContext _context;
        private readonly UserManager<User> _userManager;
        public WishlistsController(PhoneshopIdentityContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Wishlist>>> GetWishlist()
        {
            // Lấy thông tin người dùng từ User.Identity
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userId = user.Id;
            //var user = _userManager.FindByNameAsync(username).Result

            var wishlistItems = await _context.Wishlist
                .Where(w => w.UserId == userId && w.Status)
                .Include(w => w.Phone)
                .ToListAsync();

            if (wishlistItems == null || wishlistItems.Count == 0)
            {
                return NotFound();
            }

            return Ok(wishlistItems);
        }

        [HttpGet("GetWishlist")]
        public async Task<ActionResult<IEnumerable<Wishlist>>> GetFavorites(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var favorites = await _context.Wishlist.Where(p => p.UserId == user.Id).Include(p => p.Phone).ToListAsync();


            return favorites;
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutWishlist(int id, Wishlist wishlist)
        {
            if (id != wishlist.Id)
            {
                return BadRequest();
            }
            _context.Entry(wishlist).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddWishlist([FromBody] Wishlist wishlistItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.Identity.IsAuthenticated)
            {
                var username = User.Identity.Name;
                var user = await _userManager.FindByNameAsync(username);

                if (user != null)
                {
                    var userId = user.Id;

                    // Kiểm tra xem mục đã tồn tại trong Wishlist chưa
                    var existingItem = _context.Wishlist
                        .FirstOrDefault(w => w.UserId == userId && w.PhoneId == wishlistItem.PhoneId);

                    if (existingItem == null)
                    {
                        // Thêm mục vào Wishlist nếu chưa tồn tại
                        var wishlist = new Wishlist
                        {
                            PhoneId = wishlistItem.PhoneId,
                            UserId = userId,
                        };

                        _context.Wishlist.Add(wishlist);
                        var result = _context.SaveChangesAsync().Result;
                        if (result > 0)
                        {
                            return Ok(new { Message = "Thông tin người dùng đã được cập nhật thành công." });
                        }
                        else
                        {
                            // Xử lý lỗi nếu có
                            return BadRequest(new { Message = "Có lỗi xảy ra khi cập nhật thông tin người dùng." });
                        }
                    }
                    else
                    {
                        // Mục đã tồn tại trong Wishlist
                        return BadRequest("Mục đã tồn tại trong Wishlist.");
                    }
                }
                else
                {
                    // Không tìm thấy người dùng với tên đăng nhập tương ứng
                    return NotFound($"Không tìm thấy người dùng với tên đăng nhập {User.Identity.Name}.");
                }
            }
            else
            {
                // Người dùng chưa đăng nhập
                return Unauthorized("Người dùng chưa đăng nhập.");
            }
        }

        //[HttpPost]
        //[Authorize(Roles = "User")]
        //public async Task<IActionResult> AddToCartFromWishlist(int phoneId)
        //{
        //    // Kiểm tra xem Phone và User có tồn tại hay không
        //    var phone = await _context.Phones.FindAsync(phoneId);
        //    if (phone == null)
        //    {
        //        return NotFound("Phone không tồn tại");
        //    }


        //    var wishlistItem = await _context.Wishlist
        //        .FirstOrDefaultAsync(w => w.PhoneId == phoneId);

        //    if (wishlistItem == null)
        //    {
        //        return NotFound("Mục trong danh sách yêu thích không tồn tại");
        //    }

        //    var username = User.Identity.Name;
        //    var user = await _userManager.FindByNameAsync(username);
        //    var userid = user.Id;
        //    // Tạo mới một đối tượng CartItem từ WishlistItem
        //    var cartItem = new Cart
        //    {
        //        PhoneId = phoneId,
        //        UserId = userid,

        //        Price = phone.Price // Giả sử giá của sản phẩm giống nhau trong giỏ hàng và danh sách yêu thích
        //    };

        //    // Thêm vào DbContext và lưu thay đổi vào cơ sở dữ liệu
        //    _context.CartItems.Add(cartItem);
        //    _context.Wishlists.Remove(wishlistItem); // Xóa khỏi danh sách yêu thích sau khi thêm vào giỏ hàng
        //    await _context.SaveChangesAsync();

        //    return Ok("Đã thêm vào giỏ hàng từ danh sách yêu thích");
        //}

















        [HttpPost("CheckExists")]
        public async Task<ActionResult<bool>> WishlistExists(string username, int productId)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                // Handle the case where the user is not found.
                return NotFound();
            }

            var check = await _context.Wishlist.AnyAsync(w => w.PhoneId == productId && w.UserId == user.Id);
            return Ok(check);
        }

        [HttpPost("addFavorite")]
        public async Task<ActionResult> AddProductToFavorite(string username, int productId)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                // Handle the case where the user is not found.
                return NotFound();
            }

            var existingFavoriteItem = await _context.Wishlist
                .FirstOrDefaultAsync(f => f.UserId == user.Id && f.PhoneId == productId);

            if (existingFavoriteItem != null)
            {
                // If the product already exists, you may want to return an appropriate response.
                return Conflict("Product already exists in the wishlist.");
            }

            var newFavoriteItem = new Wishlist
            {
                UserId = user.Id,
                PhoneId = productId,
            };

            _context.Wishlist.Add(newFavoriteItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("DeleteWishList")]
        public async Task<ActionResult> DeleteWishList([FromQuery] string username, int phoneId)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                // Handle the case where the user is not found.
                return NotFound();
            }

            var existingFavoriteItem = await _context.Wishlist
                .FirstOrDefaultAsync(f => f.UserId == user.Id && f.PhoneId == phoneId);

            if (existingFavoriteItem != null)
            {
                _context.Wishlist.Remove(existingFavoriteItem);
                await _context.SaveChangesAsync();
                return Ok("Xóa Item thành công");
            }

           

            return NotFound("Product not found in the wishlist.");
        }

        [HttpDelete("DeleteAllWishList")]
        public async Task<ActionResult> DeleteAllWishList(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                // Handle the case where the user is not found.
                return NotFound();
            }

            var itemsToDelete = await _context.Wishlist
                .Where(f => f.UserId == user.Id)
                .ToListAsync();

            if (itemsToDelete.Count == 0)
            {
                // If the wishlist is already empty, return an appropriate response.
                return NoContent();
            }

            _context.Wishlist.RemoveRange(itemsToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }



    }
}