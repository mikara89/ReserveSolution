using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Player.Data.Models.Entites;
using Player.Data.Persistence;
using Player.Domains.Models;
using Player.Messanger.Sender;
using Player.Messanger.Sender.Options;
using Player.Service.Commands;
using Player.Service.Exceptions;
using Player.Service.Repository;
using Microsoft.EntityFrameworkCore;

namespace Player.Service.Hendlers
{
    public class PlayerDeleteHandler : IRequestHandler<PlayerDeleteCommand, PlayerDto>
    {
        private readonly IMapper _mapper;
        private readonly IPlayerRepository _playerRepository;
        private readonly IPlayerEventEmitter _eventSernder;
        private readonly ILogger<PlayerDeleteHandler> _logger;

        public PlayerDeleteHandler(IMapper mapper,
                                   IPlayerEventEmitter eventSernder,
                                   ILogger<PlayerDeleteHandler> logger,
                                   IPlayerRepository playerRepository)
        {
            _mapper = mapper;
            _eventSernder = eventSernder;
            _logger = logger;
            _playerRepository = playerRepository;
        }

        public async Task<PlayerDto> Handle(PlayerDeleteCommand request, CancellationToken cancellationToken)
        {
            var Player =await _playerRepository
                .FindByCondition(p=>p.Id==request.PlayerId)
                .Include(x => x.Infos)
                .FirstOrDefaultAsync();

            if (Player == null)
            {
                _logger.LogInformation("Player don't exist.");
                throw new NotExistException();
            }
            if (request.UserId != Player.UserId || !request.IsSuperUser)
            {
                _logger.LogInformation("Player can't be deleted if user is not owner or SuperUser.");
                throw new NotAllowedException("Player can't be deleted if user is not owner or SuperUser.");
            }
            var Info = Player.LastInfo();

            Player.Infos.Add(new PlayerInfoEntity
            {
                FullName= Info.FullName,
                NickName = Info.NickName,
                IsActive=false
            });

            try
            {
                _playerRepository.Update(Player);
                await _playerRepository.SaveAsync();
                _eventSernder.Send(Player, TopicType.PlayerDeletedTopic);
                _logger.LogInformation("Player deleted" + (request.IsSuperUser ? " by SuperUser." : " by owner."));
                return _mapper.Map<PlayerEntity, PlayerDto>(Player);
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Player didn't deleted.", ex);
                throw;
            }
            
        }
    }
}
