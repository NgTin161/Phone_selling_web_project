using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    [NotMapped]
    public class ResetPassword
    {
        [Required]
        public string Password { get; set; } = null!;

        [Compare("Password", ErrorMessage = "The password and confirmation password don't match. ")]
        public string ConfirmPassword { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Token { get; set; } = null!;
    }
}
