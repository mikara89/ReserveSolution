using System.ComponentModel.DataAnnotations;

namespace SendMailService.Messaging.Receive.Models
{
    public class MailModel 
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public MailCredentials  credentials { get; set; }
    }
}
