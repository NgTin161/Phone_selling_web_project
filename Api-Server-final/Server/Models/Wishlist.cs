using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class Wishlist
    {
        public int Id { get; set; }

        public int PhoneId { get; set; }

        public Phone Phone { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]

        public User User { get; set; }

        [DefaultValue(true)]

        public bool Status { get; set; }

        public Wishlist()
        {
            Status = true;
        }
    }
}
