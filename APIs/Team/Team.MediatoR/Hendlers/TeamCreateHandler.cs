using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Team.Data.Models.Entites;
using Team.Data.Persistence;
using Team.Domains.Models;
using Team.Messanger.Sender;
using Team.Messanger.Sender.Options;
using Team.Service.Commands;
using Team.Service.Exceptions;

namespace Team.Service.Hendlers
{
    public class TeamCreateHandler : IRequestHandler<TeamCreateCommand, TeamDto>
    {
        private readonly IMapper _mapper;
        private readonly TeamDBContext _context;
        private readonly ITeamEventEmitter _eventSernder;
        private readonly ILogger<TeamCreateHandler> _logger;

        public TeamCreateHandler(IMapper mapper,
                                 TeamDBContext context,
                                 ITeamEventEmitter eventSernder,
                                 ILogger<TeamCreateHandler> logger)
        {
            _mapper = mapper;
            _context = context;
            _eventSernder = eventSernder;
            _logger = logger;
        }

        public async Task<TeamDto> Handle(TeamCreateCommand request, CancellationToken cancellationToken)
        {
            var team = new TeamEntity()
            {
                UserId = request.UserId
            };

            _mapper.Map(request.TeamCreate, team);

            _context.Teams.Add(team);
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Team created.");
                _eventSernder.Send(team, TopicType.TeamCreatedTopic);
                return _mapper.Map<TeamEntity, TeamDto>(team);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError("Team not created.", ex);
                throw;
            }
        }
    }
}
