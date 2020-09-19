using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Player.Service.Queries;
using Player.Data.Persistence;
using Player.Domains.Models;
using Player.Data.Models.Entites;
using Player.Service.Repository;

namespace Player.Service.Hendlers
{
    public class GetAllPlayersHandler : IRequestHandler<GetAllPlayersQuery, List<PlayerDto>>
    {
        private readonly IMapper _mapper;
        private readonly IPlayerRepository _playerRepository;

        public GetAllPlayersHandler(IMapper mapper, IPlayerRepository playerRepository)
        {
            _mapper = mapper;
            _playerRepository = playerRepository;
        }
        public async Task<List<PlayerDto>> Handle(GetAllPlayersQuery request, CancellationToken cancellationToken)
        {
            var Players = await _playerRepository
                .FindAll()
                .Include(x => x.Infos)
                .ToListAsync();
            return _mapper.Map<List<PlayerEntity>, List<PlayerDto>>(Players);
        }
    }
    
}
