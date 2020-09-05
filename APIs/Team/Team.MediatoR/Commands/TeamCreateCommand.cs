using MediatR;
using Team.Domains.Models;

namespace Team.Service.Commands
{
    public class TeamCreateCommand:IRequest<TeamDto>
    {
        public TeamCreate TeamCreate { get; }
        public string UserId { get; set; }

        public TeamCreateCommand(TeamCreate teamCreate, string userId)
        {
            TeamCreate = teamCreate;
            UserId = userId;
        }

    }
}
