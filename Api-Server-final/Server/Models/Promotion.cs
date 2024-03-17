using System.ComponentModel;

namespace Server.Models
{
    public class Promotion
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string code { get; set; }

        [DefaultValue(0)]
        public int DiscountAmount { get; set; }

        public double DiscountPercentage { get; set; }

        [DefaultValue(true)]
        public bool Status { get; set; }

        public Promotion() {
            Status = true;
        }
    }
}
