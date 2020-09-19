using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Player.Data.Models.Entites;
using Player.Domains.Models;

namespace Player.Service.Queries
{
    public class GetAllPlayersQuery:IRequest<List<PlayerDto>> 
    {

    }
}
