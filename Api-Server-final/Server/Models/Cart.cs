using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace Server.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public int PhoneModelId { get; set; }

        [ForeignKey("PhoneModelId")]
        public PhoneModel PhoneModel { get; set; }

        [DefaultValue(1)]
        public int Quantity { get; set; }

        public Cart()
        {
            Quantity = 1;
        }
    }
}
