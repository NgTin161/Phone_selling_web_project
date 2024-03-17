using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    [NotMapped]
    public class Filter
    {
        public string Cpu { get; set; }
        public int Ram { get; set; }
        public double ScreenSize { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
