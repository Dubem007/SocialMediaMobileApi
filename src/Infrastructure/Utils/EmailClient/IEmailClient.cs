using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Utils.EmailClient
{
    public interface IEmailClient
    {
        Task SendEmailAsync(string receiverAddress, string subject, string body);
        void SendSingleEmail(string receiverAddress, string message, string subject);
    }
}
