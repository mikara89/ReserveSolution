using MediatR;
using Player.Domains.Models;

namespace Player.Service.Commands 
{
    public class PlayerCreateCommand:IRequest<PlayerDto>
    {
        public PlayerCreate PlayerCreate { get; } 
        public string UserId { get; set; }
        public string InvitationId { get; set; }

        public PlayerCreateCommand(PlayerCreate playerCreate, string userId,string invitationId)
        {
            PlayerCreate = playerCreate;
            UserId = userId;
            InvitationId = invitationId;
        }

    }
}
