using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Team.Data.Models.Entites;
using Team.Domains.Models;

namespace Team.Service.Queries
{
    public class GetAllTeamsQuery:IRequest<List<TeamDto>> 
    {

    }
}
