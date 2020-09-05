using MediatR;
using Team.Domains.Models;

namespace Team.Service.Commands
{
    public class TeamUpdateCommand : IRequest<TeamDto>
    {

        public string TeamId { get; }
        public TeamUpdate TeamUpdate { get; }
        public string UserId { get; set; } 

        public TeamUpdateCommand(string teamId, TeamUpdate teamUpdate, string userId)
        {
            TeamId = teamId;
            TeamUpdate = teamUpdate; 
            UserId = userId;
        }
    }
}
