using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models.Momo
{
    [NotMapped]
    public class MomoOptionModel
    {
        public string StoreName { get; set; }
        public string MomoApiUrl { get; set; }
        public string SecretKey { get; set; }
        public string AccessKey { get; set; }
        public string ReturnUrl { get; set; }
        public string NotifyUrl { get; set; }
        public string PartnerCode { get; set; }
        public string RequestType { get; set; }
    }
}
