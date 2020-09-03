namespace SendMailService.Messaging.Receive.Models
{
    public class MailCredentials
    {
        public int Port { get; set; }

        public string Host { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }
    }
}
