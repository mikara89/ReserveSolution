using MediatR;
using System.Collections.Generic;
using Player.Domains.Models;

namespace Player.Service.Queries
{
    public class GetPlayerByIdQuery : IRequest<PlayerDto> 
    {
        public readonly string PlayerId;

        public GetPlayerByIdQuery(string id)
        {
            this.PlayerId = id;
        }
    }
}
