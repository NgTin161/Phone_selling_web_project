using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class Review
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public int PhoneIdModelId { get; set; }

        public PhoneModel PhoneModel { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public DateTime RatingDate { get; set; }

        [DefaultValue(true)]
        public bool Status { get; set; }

        public Review() { 
            Status = true;
        }
    }
}
