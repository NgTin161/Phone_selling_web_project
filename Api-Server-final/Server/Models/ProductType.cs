using System.ComponentModel;

namespace Server.Models
{
    public class ProductType
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [DefaultValue(true)]
        public bool Status { get; set; }

        public ProductType() { 
            Status = true;
        }
    }
}
