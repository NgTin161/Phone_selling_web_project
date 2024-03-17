using System.ComponentModel;

namespace Server.Models
{
    public class PromotionDetail
    {
        public int Id { get; set; }

        public int PhoneModelId { get; set; }

        public PhoneModel PhoneModel { get; set; }

        public int PromotionId { get; set; }

        public Promotion Promotion { get; set; }

        [DefaultValue(0)]
        public int DiscountAmount { get; set; }

        public double DiscountPercentage { get; set; }
    }
}
