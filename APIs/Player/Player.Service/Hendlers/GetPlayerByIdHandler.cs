using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Player.Data.Models.Entites;
using Player.Data.Persistence;
using Player.Domains.Models;
using Player.Service.Queries;
using Player.Service.Repository;
using Microsoft.EntityFrameworkCore;

namespace Player.Service.Hendlers
{
    public class GetPlayerByIdHandler : IRequestHandler<GetPlayerByIdQuery, PlayerDto>
    {
        private readonly IMapper _mapper;
        private readonly IPlayerRepository _playerRepository;

        public GetPlayerByIdHandler(IMapper mapper, IPlayerRepository playerRepository)
        {
            _mapper = mapper;
            _playerRepository = playerRepository;
        }

        public async Task<PlayerDto> Handle(GetPlayerByIdQuery request, CancellationToken cancellationToken)
        {
            var Player = await _playerRepository
                .FindByCondition(p => p.Id == request.PlayerId)
                .Include(x => x.Infos)
                .FirstOrDefaultAsync();
            return _mapper.Map<PlayerEntity, PlayerDto>(Player);
        }
    }
}
