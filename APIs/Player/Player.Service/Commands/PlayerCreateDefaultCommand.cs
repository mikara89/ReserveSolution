using MediatR;
using Player.Domains.Models;

namespace Player.Service.Commands
{
    public class PlayerCreateDefaultCommand : IRequest<PlayerDto> 
    {
        public PlayerCreate PlayerCreate { get; }
        public string UserId { get; }
        public string TeamId { get; }

        public PlayerCreateDefaultCommand(PlayerCreate playerCreate, string userId, string teamId)
        {
            PlayerCreate = playerCreate;
            UserId = userId;
            TeamId = teamId;
        }

    }
}
