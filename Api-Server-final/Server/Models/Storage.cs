using System.ComponentModel;

namespace Server.Models
{
    public class Storage
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [DefaultValue(true)]
        public bool Status { get; set; }

        public Storage() { 
            Status = true;
        }    
    }
}
