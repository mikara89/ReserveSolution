using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Player.Messanger.Sender.Options;
using Newtonsoft.Json;

namespace Player.Messager.Receiver
{
    public class MessageReceiver : BackgroundService
    {
        private IModel _channel;
        private IConnection _connection;
        private string _queueName;
        private readonly ILogger<MessageReceiver> _logger;
        private readonly RabbitMqSettings _rabbitMqOptions;
        private readonly IMessageManager  _messageManager;
        public MessageReceiver(
            ILogger<MessageReceiver> logger,
            IOptions<RabbitMqSettings> rabbitMqOptions,
            IMessageManager messageManager)
        {
            this._rabbitMqOptions = rabbitMqOptions.Value;
            this._logger = logger;

            Init();
            _messageManager = messageManager;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += OnReceived;

            consumer.Shutdown += OnShutdown;
            consumer.Registered += OnRegistered;
            consumer.Unregistered += OnUnregistered;
            consumer.ConsumerCancelled += OnCancelled;

            _channel.BasicConsume(_queueName, false, consumer);
            return Task.CompletedTask;
        }

        private void OnCancelled(object sender, ConsumerEventArgs e)
        {
        }

        private void OnUnregistered(object sender, ConsumerEventArgs e)
        {
        }

        private void OnRegistered(object sender, ConsumerEventArgs e)
        {
        }

        private void OnShutdown(object sender, ShutdownEventArgs e)
        {
        }

        private void OnReceived(object sender, BasicDeliverEventArgs e)
        {
            var content = Encoding.UTF8.GetString(e.Body);
            var model = JsonConvert.DeserializeObject<object>(content);
            _messageManager.Handler(model,e.RoutingKey);
        }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
        }
        private void Init()
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqOptions.Hostname,
                UserName = _rabbitMqOptions.UserName,
                Password = _rabbitMqOptions.Password
            };
            _connection = factory.CreateConnection();
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: _rabbitMqOptions.ExchangeName,
                                  type: "topic");

            _queueName = _channel.QueueDeclare().QueueName;

            foreach (var routingKey in _rabbitMqOptions.ReceiverTopicRoutingKeys)
            {
                _channel.QueueBind(queue: _queueName,
                                  exchange: _rabbitMqOptions.ExchangeName,
                                  routingKey: routingKey);
            }

            _logger.LogInformation("Endpoint for rabbitMq: " + _connection.Endpoint.ToString());
        }
        public override void Dispose()
        {
            if (_channel != null)
                _channel.Close();
            if (_connection != null)
                _connection.Close();
            base.Dispose();
        }
    }
}
