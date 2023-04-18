using MailKit.Net.Smtp;
using Domain.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using CorePush.Apple;
using MimeKit.Text;
using Hangfire;

namespace Infrastructure.Utils.EmailClient
{
    public class EmailClient : IEmailClient
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailClient(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public void SendSingleEmail(string receiverAddress, string message, string subject)
        {
            BackgroundJob.Enqueue(() => SendEmailAsync(receiverAddress, message, subject));
        }

        public async Task SendEmailAsync(string email, string body, string subject)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = subject;
                message.Body = new TextPart(TextFormat.Html) { Text = body };
                
                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, true);
                  
                    await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }
    }
}
