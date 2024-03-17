using System.ComponentModel;

namespace Server.Models
{
    public class ComboDetail
    {
        public int Id { get; set; }

        public int PhoneId { get; set; }

        public Phone Phone { get; set; }

        public int ComboId { get; set; }

        public Combo Combo { get; set; }

        [DefaultValue(1)]
        public int Quantity { get; set; }

        [DefaultValue(0)]
        public int UnitPrice { get; set; }

        public ComboDetail()
        {
            Quantity = 1;
            UnitPrice = 0;
        }
    }
}
