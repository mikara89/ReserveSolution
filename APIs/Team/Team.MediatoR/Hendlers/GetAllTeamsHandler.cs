using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Team.Data.Models.Entites;
using Team.Data.Persistence;
using Team.Domains.Models;
using Team.Service.Queries;

namespace Team.Service.Hendlers
{
    public class GetAllTeamsHandler : IRequestHandler<GetAllTeamsQuery, List<TeamDto>>
    {
        private readonly IMapper _mapper;
        private readonly TeamDBContext _context;

        public GetAllTeamsHandler(IMapper mapper,TeamDBContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<List<TeamDto>> Handle(GetAllTeamsQuery request, CancellationToken cancellationToken)
        {
            var teams = await _context.Teams.ToListAsync();
            return _mapper.Map<List<TeamEntity>, List<TeamDto>>(teams);
        }
    }
    
}
