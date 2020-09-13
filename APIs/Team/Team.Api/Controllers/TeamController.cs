using System;
using System.Threading.Tasks;
using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Team.Api.Options;
using Team.Domains.Models;
using Team.Service.Commands;
using Team.Service.Exceptions;
using Team.Service.Queries;

namespace Team.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly IAuthorizationService _authorizService;
        private string UserId=> User.FindFirst(JwtClaimTypes.Subject).Value;
        public TeamController(IMediator mediator,IAuthorizationService authorizationService)
        {
            _mediator = mediator;
            _authorizService = authorizationService;
        }

        // GET: api/Team
        [HttpGet]
        public async Task<IActionResult> GetTeams()
        {
            var query = new GetAllTeamsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
         
        }

        // GET: api/Team/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeam(string id)
        {
            var query = new GetTeamByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
            
        }

        // PUT: api/Team/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeam(string id, TeamUpdate teamUpdate)
        {
            var command = new TeamUpdateCommand(id, teamUpdate, UserId);
            try 
            {
                var result = await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex is NotExistException)
                    return NotFound();
                else
                    return BadRequest();
                throw;
            }
        }

        // POST: api/Team
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<IActionResult> PostTeam(TeamCreate teamCreate)
        {
            var command = new TeamCreateCommand(teamCreate, UserId);
            var result = await _mediator.Send(command);
            return CreatedAtAction("GetTeam", new { id = result.Id }, result);
        }

        // DELETE: api/Team/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(string id)
        {
            var IsSuperUser = (await _authorizService.AuthorizeAsync(User, ClaimPolicy.SuperUserClaimPolicy)).Succeeded;
            var query = new TeamDeleteCommand(id, UserId, IsSuperUser);
            
            try
            {
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                if (ex is NotAllowedException)
                    return Forbid(ex.Message);
                else if (ex is NotExistException)
                    return NotFound();
                else
                    return BadRequest();
                throw;
            }
        }
    }
}
