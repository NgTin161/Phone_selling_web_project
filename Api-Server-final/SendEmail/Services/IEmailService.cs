using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SendEmail.Models;
namespace SendEmail.Services
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
