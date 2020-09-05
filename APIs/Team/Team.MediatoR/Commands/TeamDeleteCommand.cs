using MediatR;
using Team.Domains.Models;

namespace Team.Service.Commands
{
    public class TeamDeleteCommand : IRequest<TeamDto> 
    {
        public string TeamId { get; }
        public string UserId { get; }
        public bool IsSuperUser { get; }

        public TeamDeleteCommand(string teamId, string UserId, bool IsSuperUser)
        {
            TeamId = teamId;
            this.UserId = UserId;
            this.IsSuperUser = IsSuperUser;
        }
    }
}
