using MediatR;
using System.Collections.Generic;
using Team.Domains.Models;

namespace Team.Service.Queries
{
    public class GetTeamByIdQuery : IRequest<TeamDto> 
    {
        public readonly string TeamId;

        public GetTeamByIdQuery(string id)
        {
            this.TeamId = id;
        }
    }
}
