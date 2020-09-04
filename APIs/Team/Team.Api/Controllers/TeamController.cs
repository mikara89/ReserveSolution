using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Team.Api.Models;
using Team.Api.Options;
using Team.Data.Models.Entites;
using Team.Data.Persistence;

namespace Team.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly TeamDBContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TeamController> _logger;
        private readonly IAuthorizationService authorizationService;

        public TeamController(TeamDBContext context,IMapper mapper,ILogger<TeamController> logger,IAuthorizationService authorizationService)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            this.authorizationService = authorizationService;
        }

        // GET: api/Team
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetTeams()
        {
            

            var teams = await _context.Teams.ToListAsync();
            return _mapper.Map<List<TeamEntity>, List<TeamDto>>(teams);
        }

        // GET: api/Team/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDto>> GetTeam(string id)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team == null)
            {
                return NotFound();
            }

            return _mapper.Map<TeamEntity, TeamDto>(team);
        }

        // PUT: api/Team/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeam(string id, TeamUpdate teamUpdate)
        {

            if (TeamRegNumberExists(teamUpdate.RegNumber))
            {
                _logger.LogInformation("Registration number already taken.");
                return Conflict("Registration number already taken.");
            }
                
            if (TeamNameExists(teamUpdate.TeamName))
            {
                _logger.LogInformation("Registration team name already taken.");
                return Conflict("Registration team name already taken.");
            }

            var team = _context.Teams.FirstOrDefault(t => t.Id == id);
            var userId = User.FindFirst(JwtClaimTypes.Subject).Value;

            if (team == null || userId!= team.UserId)
                return BadRequest();

            _context.Entry(team).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Team created");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // POST: api/Team
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<TeamDto>> PostTeam(TeamCreate teamCreate)
        {
            if (TeamRegNumberExists(teamCreate.RegNumber))
            {
                _logger.LogInformation("Registration number already taken.");
                return Conflict("Registration number already taken.");
            }

            if (TeamNameExists(teamCreate.TeamName))
            {
                _logger.LogInformation("Registration team name already taken.");
                return Conflict("Registration team name already taken.");
            }


            var team = new TeamEntity() { 
                UserId= User.FindFirst(JwtClaimTypes.Subject).Value
            };

            _mapper.Map(teamCreate, team);

            _context.Teams.Add(team);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TeamExists(team.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTeam", new { id = team.Id }, _mapper.Map<TeamEntity, TeamDto>(team));
        }

        // DELETE: api/Team/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TeamDto>> DeleteTeam(string id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                _logger.LogInformation("Team don't exist.");
                return NotFound();
            }

            var userId = User.FindFirst(JwtClaimTypes.Subject).Value;
            var IsSuperUser = (await authorizationService.AuthorizeAsync(User, ClaimPolicy.SuperUserClaimPolicy)).Succeeded;

            if (userId!= team.UserId || !IsSuperUser)
            {
                _logger.LogInformation("Team can't be deleted if user is not owner or SuperUser.");
                return Forbid("Team can't be deleted if user is not owner or SuperUser.");
            }
                
            _context.Teams.Remove(team);

            await _context.SaveChangesAsync();
            _logger.LogInformation("Team deleted"+(IsSuperUser?" by SuperUser.": " by owner."));

            return _mapper.Map<TeamEntity, TeamDto>(team);
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
