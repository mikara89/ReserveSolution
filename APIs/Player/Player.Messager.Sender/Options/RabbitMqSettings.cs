namespace Player.Messanger.Sender.Options
{
    public class RabbitMqSettings
    {
        public string Hostname { get;  set; }
        public string ExchangeName { get;  set; }
        public string[] ReceiverTopicRoutingKeys { get; set; }
        public string UserName { get;  set; }
        public string Password { get;  set; }
    }
}

