using IdentityService.Messaging.Options;
using IdentityService.Messaging.Sender.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Messaging.Sender.Sender
{
    public class MailSendSender
    {
        private readonly string _hostname;
        private readonly string _queueName;
        private readonly string _username;
        private readonly string _password;
        private readonly ILogger<MailSendSender> logger;

        public MailSendSender(IOptions<RabbitMqConfiguration> rabbitMqOptions,ILogger<MailSendSender> logger )
        {
            _hostname = rabbitMqOptions.Value.Hostname;
            _queueName = rabbitMqOptions.Value.QueueName;
            _username = rabbitMqOptions.Value.UserName;
            _password = rabbitMqOptions.Value.Password;
            this.logger = logger;
        }

        public void Send(MailModel model)
        {
            logger.LogInformation("Sending confim email link to user");
            var factory = new ConnectionFactory() { HostName = _hostname, UserName = _username, Password = _password };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                logger.LogInformation("Declering queue: "+ _queueName);
                channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                var json = JsonConvert.SerializeObject(model);
                var body = Encoding.UTF8.GetBytes(json);
                
                channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
                logger.LogInformation("Message added to queue: " + _queueName);
            }
        }
    }
}
