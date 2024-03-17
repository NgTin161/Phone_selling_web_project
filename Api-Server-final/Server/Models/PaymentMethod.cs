using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string nameMethod { get; set; }

        [DefaultValue(true)]
        public bool Status { get; set; }

        public PaymentMethod() {
            Status = true;
        }
    }
}
