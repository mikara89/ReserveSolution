using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using Team.Data.Models.Entites;
using Team.Messanger.Sender.Options;

namespace Team.Messanger.Sender
{
    public interface ITeamEventEmitter
    {
        void Send(TeamEntity model,TopicType topicType);
    }
    public class TeamEventEmitter : ITeamEventEmitter
    {
        private readonly string _hostname;
        private readonly string _exchangeName;
        private readonly string _username;
        private readonly string _password;
        private readonly ILogger<TeamEventEmitter> logger;

        public TeamEventEmitter(IOptions<RabbitMqSettings> rabbitMqOptions, ILogger<TeamEventEmitter> logger)
        {
            _hostname = rabbitMqOptions.Value.Hostname;
            _exchangeName = rabbitMqOptions.Value.ExchangeName;
            _username = rabbitMqOptions.Value.UserName;
            _password = rabbitMqOptions.Value.Password;
            this.logger = logger;
        }

        public void Send(TeamEntity model, TopicType topicType)
        {
            logger.LogInformation("Emitting new Team created.");
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

        private static string GetRoutingKey(TopicType topicType)
        {
            return topicType
                .ToString()
                .Substring(0, topicType.ToString().Length - 5)
                .Replace("Team","Team.")
                .ToLower();
        }
    }
}

//int value = GetValueFromDb();
//var enumDisplayStatus = (EnumDisplayStatus)value;
//string stringValue = enumDisplayStatus.ToString();
