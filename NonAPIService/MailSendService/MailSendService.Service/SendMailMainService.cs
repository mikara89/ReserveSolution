using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendMailService.Service.Options;
using System.Net;
using System.Net.Mail;

namespace SendMailService.Service
{
    public class SendAppMailService : SendMailService
    {
        public SendAppMailService(IOptions<AppEmailSettings> options,ILogger<SendAppMailService> logger) : base
            (
                new SmtpClient
                {
                    Host = options.Value.Host,
                    Port = options.Value.Port,
                    UseDefaultCredentials = false,
                },
                new NetworkCredential
                {
                    Password = options.Value.Password,
                    UserName = options.Value.UserName,
                },
                new MailMessage
                {
                    From = new MailAddress(options.Value.Email),
                    IsBodyHtml=true,
                }, logger
            )
        {

        }
    }
}