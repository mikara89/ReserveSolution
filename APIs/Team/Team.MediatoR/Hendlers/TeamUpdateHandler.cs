using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Team.Data.Models.Entites;
using Team.Data.Persistence;
using Team.Domains.Models;
using Team.Service.Commands;
using Team.Service.Exceptions;

namespace Team.Service.Hendlers
{

    public class TeamUpdateHandler : IRequestHandler<TeamUpdateCommand, TeamDto>
    {
        private readonly IMapper _mapper;
        private readonly TeamDBContext _context;
        private readonly ILogger<TeamUpdateHandler> _logger;

        public TeamUpdateHandler(IMapper mapper, TeamDBContext context, ILogger<TeamUpdateHandler> logger)
        {
            _mapper = mapper;
            _context = context;
            _logger = logger;
        }

        public async Task<TeamDto> Handle(TeamUpdateCommand request, CancellationToken cancellationToken)
        {
            if (TeamRegNumberExists(request.TeamUpdate.RegNumber))
            {
                _logger.LogInformation("Registration number already taken.");
                throw new AlreadyExistException("Registration number already taken.");
            }

            if (TeamNameExists(request.TeamUpdate.TeamName))
            {
                _logger.LogInformation("Registration team name already taken.");
                throw new AlreadyExistException("Registration team name already taken.");
            }

            var team = _context.Teams.FirstOrDefault(t => t.Id == request.TeamId);

            if (team == null || request.UserId != team.UserId) 
                throw new Exception();

            _context.Entry(team).State = EntityState.Modified;

            _mapper.Map(request.TeamUpdate, team);
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Team updated.");
                return _mapper.Map<TeamEntity, TeamDto>(team);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamExists(request.TeamId))
                {
                    _logger.LogInformation("Team with given Id not exist.");
                    throw new NotExistException();
                }
                else
                {
                    throw;
                }
            }

        }

        private bool TeamExists(string id)
        {
            return _context.Teams.Any(e => e.Id == id);
        }
        private bool TeamRegNumberExists(int regNumber)
        {
            return _context.Teams.Any(e => e.RegNumber == regNumber);
        }
        private bool TeamNameExists(string teamName)
        {
            return _context.Teams.Any(e => e.TeamName == teamName);
        }
    }
}
