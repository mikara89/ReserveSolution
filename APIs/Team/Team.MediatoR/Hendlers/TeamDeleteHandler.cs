using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Team.Data.Models.Entites;
using Team.Data.Persistence;
using Team.Domains.Models;
using Team.Service.Commands;
using Team.Service.Exceptions;

namespace Team.Service.Hendlers
{
    public class TeamDeleteHandler : IRequestHandler<TeamDeleteCommand, TeamDto>
    {
        private readonly IMapper _mapper;
        private readonly TeamDBContext _context;
        private readonly ILogger<TeamDeleteHandler> _logger;

        public TeamDeleteHandler(IMapper mapper, TeamDBContext context, ILogger<TeamDeleteHandler> logger)
        {
            _mapper = mapper;
            _context = context;
            _logger = logger;
        }

        public async Task<TeamDto> Handle(TeamDeleteCommand request, CancellationToken cancellationToken)
        {
            var team = await _context.Teams.FindAsync(request.TeamId);
            if (team == null)
            {
                _logger.LogInformation("Team don't exist.");
                throw new NotExistException();
            }
            if (request.UserId != team.UserId || !request.IsSuperUser)
            {
                _logger.LogInformation("Team can't be deleted if user is not owner or SuperUser.");
                throw new NotAllowedException("Team can't be deleted if user is not owner or SuperUser.");
            }

            _context.Teams.Remove(team);

            await _context.SaveChangesAsync();
            _logger.LogInformation("Team deleted" + (request.IsSuperUser ? " by SuperUser." : " by owner."));

            return _mapper.Map<TeamEntity, TeamDto>(team);
        }
    }
}
