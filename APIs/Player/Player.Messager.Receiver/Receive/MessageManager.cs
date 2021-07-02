using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Player.Domains.Models;
using Player.Messager.Receiver;
using Player.Messager.Receiver.Models;
using Player.Service.Commands;

namespace Player.Messager.Receiver
{
    public class MessageManager : IMessageManager
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MessageManager> _logger;

        public MessageManager(
            ILogger<MessageManager> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _mediator = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IMediator>();
        }

        public async Task Handler(string content, string routingKey)
        {
            _logger.LogInformation("Handling message: " + routingKey);

            try
            {
                if (routingKey == "team.created")
                {
                    await AddFirstTeamPlayer(content);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Handling message of rout {routingKey} falid: {ex.Message}");
            }


        }

        private async Task AddFirstTeamPlayer(string content)
        {

            var team = JsonConvert.DeserializeObject<TeamModel>(content);
            if (team == null)
                throw new ArgumentNullException();

            var player = new PlayerCreate
            {
                FullName = team.TeamName,
                NickName = team.TeamName
            };

            var request = new PlayerCreateDefaultCommand(player, team.UserId, team.Id);
            await _mediator.Send(request);
        }

    }
}
