using SendMailService.Messaging.Receive.Options;
using SendMailService.Service;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SendMailService.Messaging.Receive.Models;

namespace SendMailService.Messaging.Receive.Receive
{
    public class SendMailMessageReceiver : BackgroundService
    {
        private IModel channel;
        private IConnection connection;
        private readonly ISendMailService sendAppMailService;
        private readonly ISendMailService sendMailService;
        private readonly ILogger<SendMailMessageReceiver> logger;
        private readonly RabbitMqMailConfiguration rabbitMqOptions;
        public SendMailMessageReceiver(
            SendAppMailService sendAppMailService,
            ISendMailService sendMailService,
            ILogger<SendMailMessageReceiver> logger,
            IOptions<RabbitMqMailConfiguration> rabbitMqOptions)
        {
            this.rabbitMqOptions = rabbitMqOptions.Value;
            this.sendAppMailService = sendAppMailService;
            this.sendMailService = sendMailService;
            this.logger = logger;

            Init();
            //Task.Run(() => {
            //    Policy.Handle<Exception>()
            //    .WaitAndRetry(2, r => TimeSpan.FromSeconds(3), (ex, ts) => { 
            //        Console.WriteLine("Error connecting to RabbitMQ");
            //    })
            //    .Execute(()=> {
                   
            //        Console.WriteLine("Connected to RabbitMQ");
            //    });
            //});
            
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnReceived;

            consumer.Shutdown += OnShutdown;
            consumer.Registered += OnRegistered;
            consumer.Unregistered += OnUnregistered;
            consumer.ConsumerCancelled += OnCancelled;

            //if(channel!=null)
            channel.BasicConsume(rabbitMqOptions.QueueName, false, consumer);
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
            var model = JsonConvert.DeserializeObject<MailModel>(content);

            HandleMessage(model);          
        }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
        }
        private void HandleMessage(MailModel model)
        {
            try
            {
                
                if(model.credentials==null)
                    sendAppMailService.SendMailAsync(model.To, model.Message, model.Subject);
                else
                {
                    sendMailService.SendMailSettings(
                        new System.Net.Mail.SmtpClient
                        {
                            Host = model.credentials.Host,
                            Port = model.credentials.Port,
                            EnableSsl = true,
                            UseDefaultCredentials = false,
                        },
                        new System.Net.NetworkCredential
                        {
                            Password = model.credentials.Password,
                            UserName = model.credentials.Email,
                        },
                        new System.Net.Mail.MailMessage
                        {
                            From = new System.Net.Mail.MailAddress(model.credentials.Email),
                        }) ;

                sendMailService.SendMailAsync(model.To, model.Message, model.Subject);
                }
                logger.LogInformation($"{DateTime.Now}  sending to:" + model.To);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message,ex);
                throw;
            }
           
        }

        

        private void Init()
        {
            var factory = new ConnectionFactory
            {
                HostName = rabbitMqOptions.Hostname,
                UserName = rabbitMqOptions.UserName,
                Password = rabbitMqOptions.Password
            };
            connection = factory.CreateConnection();
            connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            channel = connection.CreateModel();
            channel.QueueDeclare(
                queue: rabbitMqOptions.QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            Console.WriteLine("Endpoint for rabbitMq: "+ connection.Endpoint.ToString());
        }
        public override void Dispose()
        {
            if(channel!=null)
                channel.Close();
            if (connection != null)
                connection.Close();
            base.Dispose();
        }
    }
}
