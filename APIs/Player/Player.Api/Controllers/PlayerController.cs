using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Player.Api.Options;
using Player.Domains.Models;
using Player.Service.Commands;
using Player.Service.Exceptions;
using Player.Service.Queries;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Player.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAuthorizationService _authorizService;
        private string UserId => User.FindFirst(JwtClaimTypes.Subject).Value;
        public PlayerController(IMediator mediator, IAuthorizationService authorizService)
        {
            _mediator = mediator;
            _authorizService = authorizService;
        }
        // GET: api/<PlayerController>
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var query = new GetAllPlayersQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET api/<PlayerController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var query = new GetPlayerByIdQuery(id);

            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // POST api/<PlayerController>?invitationCode=asdas-fasa-fasfa-fsdds
        [HttpPost]
        public async Task<IActionResult> Post([FromQuery]string invitationCode, [FromBody] PlayerCreate playerCreate)
        {
            var command = new PlayerCreateCommand(playerCreate, UserId, invitationCode);
            try
            {
                var result = await _mediator.Send(command);
                return CreatedAtAction("Get", new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                if (ex is NotExistException)
                    return BadRequest(ex.Message);
                else
                    return BadRequest();
            }
            
            
        }

        // PUT api/<PlayerController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] PlayerUpdate playerUpdate)
        {
            var command = new PlayerUpdateCommand(id, playerUpdate, UserId);
            try
            {
                var result = await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex is NotExistException)
                    return NotFound();
                else if (ex is NotAllowedException)
                    return Forbid();
                else
                    return BadRequest();
                throw;
            }
        }

        // DELETE api/<PlayerController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var IsSuperUser = (await _authorizService.AuthorizeAsync(User, ClaimPolicy.SuperUserClaimPolicy)).Succeeded;
            var query = new PlayerDeleteCommand(id, UserId, IsSuperUser);

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
