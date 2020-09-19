using MediatR;
using Player.Domains.Models;

namespace Player.Service.Commands
{
    public class PlayerDeleteCommand : IRequest<PlayerDto> 
    {
        public string PlayerId { get; }
        public string UserId { get; }
        public bool IsSuperUser { get; }

        public PlayerDeleteCommand(string PlayerId, string UserId, bool IsSuperUser)
        {
            this.PlayerId = PlayerId;
            this.UserId = UserId;
            this.IsSuperUser = IsSuperUser;
        }
    }
}
