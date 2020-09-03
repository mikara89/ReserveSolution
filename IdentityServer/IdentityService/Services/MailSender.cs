using IdentityService.Messaging.Sender.Sender;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace IdentityService.Services
{
    public class MailSender:IEmailSender
    {
        public MailSender(MailSendSender mailSendSender)
        {
            MailSendSender = mailSendSender;
        }

        public MailSendSender MailSendSender { get; }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await Task.Run(()
                => MailSendSender.Send(
                    new Messaging.Sender.Options.MailModel { 
                        Message = htmlMessage, 
                        Subject = subject, 
                        To = email 
                    })
                );
        }
    }
}
