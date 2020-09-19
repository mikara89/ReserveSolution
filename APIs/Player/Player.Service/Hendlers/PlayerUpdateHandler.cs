using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
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

namespace Player.Service.Hendlers
{

    public class PlayerUpdateHandler : IRequestHandler<PlayerUpdateCommand, PlayerDto>
    {
        private readonly IMapper _mapper;

        private readonly IPlayerRepository _playerRepository;
        private readonly IPlayerEventEmitter _eventSernder;
        private readonly ILogger<PlayerUpdateHandler> _logger;

        public PlayerUpdateHandler(IMapper mapper,
                                 IPlayerEventEmitter eventSernder,
                                 ILogger<PlayerUpdateHandler> logger, 
                                 IPlayerRepository playerRepository)
        {
            _mapper = mapper;
            _eventSernder = eventSernder;
            _logger = logger;
            _playerRepository = playerRepository;
        }

        public async Task<PlayerDto> Handle(PlayerUpdateCommand request, CancellationToken cancellationToken)
        {

            var Player = await _playerRepository
                .FindByCondition(t => t.Id == request.PlayerId)
                .Include(x=>x.Infos)
                .FirstOrDefaultAsync();


            Player.Infos.Add(new PlayerInfoEntity
            {
                FullName = request.PlayerUpdate.FullName,
                NickName = request.PlayerUpdate.NickName
            });

            return await SavingChangesAsync(Player);

        }

        private async Task<PlayerDto> SavingChangesAsync(PlayerEntity Player)
        {
            try
            {
                _playerRepository.Update(Player);
                await _playerRepository.SaveAsync();
                _logger.LogInformation("Player updated.");
                _eventSernder.Send(Player, TopicType.PlayerUpdatedTopic);
                return _mapper.Map<PlayerEntity, PlayerDto>(Player);
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogError("Player didn't updated.");
                throw;
            }
        }
    }
}
