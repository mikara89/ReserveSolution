using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Player.Service.Repository;

namespace Player.Messager.Receiver
{
    public class MessageManager : IMessageManager
    {
        private readonly IInvetationRepository _invitationRepository;
        private readonly ILogger<MessageManager> _logger;

        public MessageManager(IInvetationRepository invitationRepository,
            ILogger<MessageManager> logger)
        {
            _invitationRepository = invitationRepository;
            _logger = logger;
        }

        public async Task Handler(object message, string routingKey)
        {
            _logger.LogInformation("Handling message: " + routingKey);
        }

    }
}
