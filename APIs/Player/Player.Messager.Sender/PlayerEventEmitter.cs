using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using Player.Data.Models.Entites;
using Player.Messanger.Sender.Options;

namespace Player.Messanger.Sender 
{
    public interface IPlayerEventEmitter
    {
        void Send(PlayerEntity model,TopicType topicType);
        void Send(PlayerEntity model,string invitationId, TopicType topicType);
    }
    public class PlayerEventEmitter : IPlayerEventEmitter
    {
        private readonly string _hostname;
        private readonly string _exchangeName;
        private readonly string _username;
        private readonly string _password;
        private readonly ILogger<PlayerEventEmitter> logger;

        public PlayerEventEmitter(IOptions<RabbitMqSettings> rabbitMqOptions, ILogger<PlayerEventEmitter> logger)
        {
            _hostname = rabbitMqOptions.Value.Hostname;
            _exchangeName = rabbitMqOptions.Value.ExchangeName;
            _username = rabbitMqOptions.Value.UserName;
            _password = rabbitMqOptions.Value.Password;
            this.logger = logger;
        }

        public void Send(PlayerEntity model, TopicType topicType)
        {
            //logger.LogInformation("Emitting new Player created.");
            var factory = new ConnectionFactory() { HostName = _hostname, UserName = _username, Password = _password };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                logger.LogInformation("Declering exchange: " + _exchangeName);
                channel.ExchangeDeclare(exchange: _exchangeName, type: "topic");

                var json = JsonConvert.SerializeObject(model);
                var body = Encoding.UTF8.GetBytes(json);

                var routingKey = GetRoutingKey(topicType);

                channel.BasicPublish(exchange: _exchangeName, routingKey: routingKey, basicProperties: null, body: body);
                logger.LogInformation($"Event send to exchange: {_exchangeName} as topic/routingKey: {routingKey}");
            }
        }
        public void Send(PlayerEntity model, string invitationId, TopicType topicType)
        {
            //logger.LogInformation("Emitting new Player created.");
            var factory = new ConnectionFactory() { HostName = _hostname, UserName = _username, Password = _password };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                logger.LogInformation("Declering exchange: " + _exchangeName);
                channel.ExchangeDeclare(exchange: _exchangeName, type: "topic");

                var json = JsonConvert.SerializeObject(new { model, invitationId });
                var body = Encoding.UTF8.GetBytes(json);

                var routingKey = GetRoutingKey(topicType);

                channel.BasicPublish(exchange: _exchangeName, routingKey: routingKey, basicProperties: null, body: body);
                logger.LogInformation($"Event send to exchange: {_exchangeName} as topic/routingKey: {routingKey}");
            }
        }

        private static string GetRoutingKey(TopicType topicType)
        {
            return topicType
                .ToString()
                .Substring(0, topicType.ToString().Length - 5)
                .Replace("Player","Player.")
                .ToLower();
        }
    }
}

