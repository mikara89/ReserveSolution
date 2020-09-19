using MediatR;
using Player.Domains.Models;

namespace Player.Service.Commands
{
    public class PlayerUpdateCommand : IRequest<PlayerDto> 
    {

        public string PlayerId { get; }
        public PlayerUpdate PlayerUpdate { get; }
        public string UserId { get; set; } 

        public PlayerUpdateCommand(string PlayerId, PlayerUpdate PlayerUpdate, string userId)
        {
            this.PlayerId = PlayerId;
            this.PlayerUpdate = PlayerUpdate; 
            UserId = userId;
        }
    }
}
