using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public DateTime IssueDate { get; set; }

        public int? ParentCommentId { get; set; }

        [ForeignKey("ParentCommentId")]
        public Comment ParentComment { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public int PhoneModelId { get; set; }

        // Reference navigation property cho khóa ngoại đến Product
        public PhoneModel PhoneModel { get; set; }

        [DefaultValue(true)]
        public bool Status { get; set; }
        public ICollection<Comment> Replies { get; set; }
        public Comment()
        {
            IssueDate = DateTime.Now;
            Status = true;
            Replies = new List<Comment>();
        }
    }
}
