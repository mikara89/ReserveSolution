using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SendMailService.Service
{
    public class SendMailService : ISendMailService
    {
        private SmtpClient smtpClient;
        private NetworkCredential networkCredential;
        private MailMessage mailMessage;
        private readonly ILogger<ISendMailService> logger;

        public event SendCompletedEventHandler OnMailSent;

        public SendMailService(SmtpClient smtpClient, NetworkCredential networkCredential, MailMessage mailMessage, ILogger<ISendMailService> logger)
        {
            this.smtpClient = smtpClient;
            this.networkCredential = networkCredential;
            this.mailMessage = mailMessage;
            this.logger = logger;
            smtpClient.SendCompleted += OnMailSent;
        }
        public SendMailService(ILogger<ISendMailService> logger)
        {
            this.logger = logger;
        }

        public void SendMailSettings(SmtpClient smtpClient, NetworkCredential networkCredential, MailMessage mailMessage)
        {
            this.smtpClient = smtpClient;
            this.networkCredential = networkCredential;
            this.mailMessage = mailMessage;
            smtpClient.SendCompleted += OnMailSent;
        }
        public void SendMail(string email, string message, string subject)
        {
            CheckDependencies();

            smtpClient.Credentials = networkCredential;

            mailMessage.To.Add(email);
            mailMessage.Body = message;
            mailMessage.Subject = subject;

            smtpClient.SendAsync(mailMessage, mailMessage);
            logger.LogInformation("Mail sent to: " + email);

        }

        private void CheckDependencies()
        {
            if (smtpClient == null)
                throw new Exception("SMPT Client is not initialized");
            if (networkCredential == null)
                throw new Exception("Network Credential is not initialized");
            if (mailMessage == null)
                throw new Exception("Mail Message is not initialized");
        }

        public async Task SendMailAsync(string email, string message, string subject)
        {
            CheckDependencies();
            smtpClient.Credentials = networkCredential;

            mailMessage.To.Add(email);
            mailMessage.Body = message;
            mailMessage.Subject = subject;
            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                logger.LogWarning("Mail sent faild: "+ex.Message,ex);
                throw;
            }
            
            logger.LogInformation("Mail sent to: " + email);
        }
    }
}