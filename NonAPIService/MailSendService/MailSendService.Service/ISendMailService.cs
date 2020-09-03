using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SendMailService.Service
{
    public interface ISendMailService
    {
        void SendMail(string email, string message, string subject);
        void SendMailSettings(SmtpClient smtpClient, NetworkCredential networkCredential, MailMessage mailMessage);
        Task SendMailAsync(string email, string message, string subject);
        event SendCompletedEventHandler OnMailSent;
    }
}