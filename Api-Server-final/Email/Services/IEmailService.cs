using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Email.Models;
namespace Email.Services
{
    internal interface IEmailService
    {
        void SendEmail(Message message) => throw new NotImplementedException();
    }
}
