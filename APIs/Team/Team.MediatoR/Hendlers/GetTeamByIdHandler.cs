using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Team.Data.Models.Entites;
using Team.Data.Persistence;
using Team.Domains.Models;
using Team.Service.Queries;

namespace Team.Service.Hendlers
{
    public class GetTeamByIdHandler : IRequestHandler<GetTeamByIdQuery, TeamDto>
    {
        private readonly IMapper _mapper;
        private readonly TeamDBContext _context;

        public GetTeamByIdHandler(IMapper mapper, TeamDBContext context)
        {
            _mapper = mapper;
            _context = context;

        }

        public async Task<TeamDto> Handle(GetTeamByIdQuery request, CancellationToken cancellationToken)
        {
            var team = await _context.Teams.FindAsync(request.TeamId);
            return _mapper.Map<TeamEntity, TeamDto>(team);
        }
    }
}
