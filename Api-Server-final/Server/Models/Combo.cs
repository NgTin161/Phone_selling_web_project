using System.ComponentModel;

namespace Server.Models
{
    public class Combo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [DefaultValue(0)]
        public int Price { get; set; }

        [DefaultValue(true)]
        public bool Status { get; set; }

        public Combo() { 
            Status = true;
        }
    }
}
