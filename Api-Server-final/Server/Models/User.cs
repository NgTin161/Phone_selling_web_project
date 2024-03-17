using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace Server.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }

        public DateTime Birthday { get; set; }

        public bool IsDeleted { get; set; }

    }
}
