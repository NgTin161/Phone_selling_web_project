using System.ComponentModel;

namespace Server.Models
{
    public class Phone
    {
        public int Id { get; set; }

        public string SKU { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [DefaultValue(0)]
        public int Stock { get; set; }

        public int ProductTypeId { get; set; }

        public ProductType ProductType { get; set; } 
        
        public string Thumbnail { get; set; }
        public ICollection<Image> Image { get; set; }

        [DefaultValue(true)]
        public bool Status { get; set; }

        public Phone() {
            Status = true;
        }
    }
}
