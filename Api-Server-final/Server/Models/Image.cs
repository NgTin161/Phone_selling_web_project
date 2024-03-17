using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Server.Models
{
    public class Image
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int PhoneId { get; set; }

        [ForeignKey("PhoneId")]
        public Phone Phone { get; set; }

        [AllowNull]
        public int? ColorId {  get; set; }
        
        [ForeignKey("ColorId")]
        public Color Color { get; set; }
        [DefaultValue(true)]
        public bool Status { get; set; }

        public Image()
        {
            Status = true;
        }
    }
}
