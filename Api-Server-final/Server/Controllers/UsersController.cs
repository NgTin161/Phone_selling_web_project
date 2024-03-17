using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using SendEmail.Models;
using SendEmail.Services;
using Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Server;
using System.Security.Policy;
using NuGet.Common;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Net.Mail;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json.Linq;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly PhoneshopIdentityContext _context;

        public UsersController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IEmailService emailService, PhoneshopIdentityContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();

            var usersWithRoles = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add(new
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = roles,
                    IsDelete = user.IsDeleted
                });
            }

            return Ok(usersWithRoles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpGet]
        [Route("testemail")]
        public async Task<IActionResult> TestEmail()
        {
            var message = new Message(new string[] { "tinnguyentrung2002@gmail.com" }, "Test email", "This is the content from our email.");
            _emailService.SendEmail(message);
            return Ok(new
            {
                Status = "Success",
                Message = "Email Sent Succcessfully"
            });

        }

        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    //    return Ok(new
                    //    {
                    //        Status = "Success",
                    //        Message = "Email Verified Succcessfully"
                    //    });
                    return Redirect("http://localhost:3000/confirmemail?status=success");
                }
            }
            return Redirect("http://localhost:3000/confirmemail?status=failed");
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([Bind("Username,Password")] LoginModel account)
        {
            var user = await _userManager.FindByNameAsync(account.Username);

            if (user != null)
            {
                if (await _userManager.IsLockedOutAsync(user))
                {
                    return Unauthorized("Tài khoản của bạn đã bị khóa. Vui lòng thử lại sau.");
                }

                if (await _userManager.CheckPasswordAsync(user, account.Password))
                {
                    await _userManager.ResetAccessFailedCountAsync(user);

                    var userRoles = await _userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        expires: DateTime.Now.AddHours(3),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
                }
                else
                {
                    await _userManager.AccessFailedAsync(user);

                    // Kiểm tra xem số lần thất bại có vượt quá giới hạn không
                    if (await _userManager.GetAccessFailedCountAsync(user) > 2)
                    {
                        await _userManager.SetLockoutEnabledAsync(user, true);
                        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddMinutes(5)); // Thời gian khoá tài khoản (5 phút)

                        return Unauthorized("Tài khoản của bạn đã bị khóa. Vui lòng thử lại sau.");
                    }
                    else
                    {
                        return Unauthorized("Sai tên đăng nhập hoặc mật khẩu. Lần thử đăng nhập thứ " + (await _userManager.GetAccessFailedCountAsync(user)));
                    }
                }
            }

            return NotFound("Sai tên đăng nhập hoặc mật khẩu.");
        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(string Username, string Password, string Email)
        {
            var userExists = await _userManager.FindByNameAsync(Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            User user = new User()
            {
                Email = Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = Username
  
            };
            var result = await _userManager.CreateAsync(user, Password);
            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // Thêm HTML vào nội dung email
                var confirmationLink = $"<a href='{Url.Action(nameof(ConfirmEmail), "Users", new { token, email = user.Email }, Request.Scheme)}'>Xác nhận tài khoản</a>";

                // Tạo đối tượng Message với nội dung HTML
                var message = new Message(new string[] { user.Email! }, "Xác nhận tài khoản - PhoneShop", confirmationLink, isHtml: true);

                _emailService.SendEmail(message);
                if (!await _roleManager.RoleExistsAsync("User"))
                        await _roleManager.CreateAsync(new IdentityRole("User"));

                if (await _roleManager.RoleExistsAsync("User"))
                {
                    await _userManager.AddToRoleAsync(user, "User");
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok();
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin(string Username, string Password, string Email, string FullName)
        {
            var userExists = await _userManager.FindByNameAsync(Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            User user = new User()
            {
                Email = Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = Username,
                FullName = FullName
            };
            var result = await _userManager.CreateAsync(user, Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError);

            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new IdentityRole("User"));

            if (await _roleManager.RoleExistsAsync("Admin"))
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }

            return Ok();
        }

        [HttpDelete("{username}")]
        public async Task<IActionResult> DeleteUser(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) { return NotFound(); }

            user.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetPasswordLink = Url.Action(nameof(ResetPassword), "Users", new { token, email = user.Email }, Request.Scheme);
                var resetPasswordLink1 = $"Nhấn vào link sau để thay đổi mật khẩu http://localhost:3000/resetpassword?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";
                var message = new Message(new string[] { user.Email! }, "Quên mật khẩu", resetPasswordLink1);
                _emailService.SendEmail(message);
                return StatusCode(StatusCodes.Status200OK,
                    new
                    {
                        Status = "Success",
                        Message = $"Password Changed request is sent on Email  {user.Email} Successlly. Please open your email & click into link  ",
                    });
            }
            return StatusCode(StatusCodes.Status400BadRequest,
                 new
                 {
                     Status = "Error",
                     Message = $"Couldn't send link to email, please try again late !",
                 });
        }

        [HttpGet]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            var model = new ResetPassword { Token = token, Email = email };
            return Ok(new
            {
                model
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassword resetPassword )
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if ( user != null )
            {
                var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
                if ( !resetPassResult.Succeeded )
                {
                    foreach ( var error in resetPassResult.Errors )
                    {
                        ModelState.AddModelError( error.Code , error.Description );
                    }   
                    return Ok(ModelState);
                }

                return StatusCode(StatusCodes.Status200OK,
                    new 
                    {
                        Status = "Success",
                        Message = $"Password has been changed"
                    });
            }
            return StatusCode(StatusCodes.Status400BadRequest,
                  new
                  {
                      Status = "Error",
                      Message = $"Couldn't send link to email, please try again."
                  });

        }



        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Đăng xuất người dùng
            await HttpContext.SignOutAsync();

            // Trả về kết quả thành công
            return Ok(new { message = "Đăng xuất thành công" });
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        [Route("user-infor")]
        public IActionResult GetUserInfo()
        {
            if (!User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "User"))
            {
                return Forbid();
            }
            var username = User.Identity.Name;
            var user = _userManager.FindByNameAsync(username).Result;

            if (user == null)
            {
                return NotFound();
            }
            var userInfo = new
            {
                UserName = username ?? "",
                PhoneNumber = user.PhoneNumber?.ToString() ?? "",
                FullName = user.FullName ?? "",
                Email = user.Email ?? "",
                BirthDay = user.Birthday != DateTime.MinValue ? user.Birthday.ToString("dd-MM-yyyy") : "",
            };


            return Ok(userInfo);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        [Route("update-user-infor")]
        public IActionResult UpdateUserInfo([FromBody] User updateUser)
        {
            if (!User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "User"))
            {
                return Forbid(); 
            }
            var username = User.Identity.Name;

            var user = _userManager.FindByNameAsync(username).Result;

            if (user == null)
            {
                return NotFound(); 
            }


            // Cập nhật thông tin người dùng
            user.FullName = updateUser.FullName;
            user.PhoneNumber = updateUser.PhoneNumber;
            user.Birthday =updateUser.Birthday;


            // Lưu thay đổi vào cơ sở dữ liệu
            var result = _userManager.UpdateAsync(user).Result;

            if (result.Succeeded)
            {
                return Ok(new { Message = "Thông tin người dùng đã được cập nhật thành công." });
            }
            else
            {
                // Xử lý lỗi nếu có
                return BadRequest(new { Message = "Có lỗi xảy ra khi cập nhật thông tin người dùng." });
            }
        }

        [HttpPost]
        [Authorize(Roles= "User")]
        [Route("changepassword")]
        public async Task<IActionResult> ChangePassword([FromQuery] string currentPassword, string newPassword)
        {
            var username = User.Identity.Name;

            var user = _userManager.FindByNameAsync(username).Result;

            if (user == null)
            {
                return NotFound("User not found");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, currentPassword);

            if (!isPasswordValid)
            {
                return BadRequest("Invalid current password");
            }

           
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (result.Succeeded)
            {
                return Ok("Password changed successfully");
            }
            else
            {
                return BadRequest("Failed to change password");
            }
        }

    }

}

