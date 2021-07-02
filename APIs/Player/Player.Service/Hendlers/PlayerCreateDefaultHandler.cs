using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Player.Data.Models.Entites;
using Player.Domains.Models;
using Player.Messanger.Sender;
using Player.Messanger.Sender.Options;
using Player.Service.Commands;
using Player.Service.Repository;
using System;

namespace Player.Service.Hendlers
{
    public class PlayerCreateDefaultHandler : IRequestHandler<PlayerCreateDefaultCommand, PlayerDto>
    {
        private readonly IMapper _mapper;
        private readonly IPlayerEventEmitter _eventSernder;
        private readonly ILogger<PlayerCreateHandler> _logger;
        private readonly IPlayerRepository _playerRepository;

        public PlayerCreateDefaultHandler(IMapper mapper,
                                 IPlayerEventEmitter eventSernder,
                                 ILogger<PlayerCreateHandler> logger,
                                 IPlayerRepository playerRepository)
        {
            _mapper = mapper;
            _eventSernder = eventSernder;
            _logger = logger;
            _playerRepository = playerRepository;
        }

        public async Task<PlayerDto> Handle(PlayerCreateDefaultCommand request, CancellationToken cancellationToken)
        {

            var player = MappingPlayerEntity(request);

            var playerDto = await SavePlayerAndRetuntDtoAsync(player);

            return playerDto;
        }



        private async Task<PlayerDto> SavePlayerAndRetuntDtoAsync(PlayerEntity player)
        {
            try
            {
                _playerRepository.Create(player);
                await _playerRepository.SaveAsync();
                _logger.LogInformation("Player created.");
                _eventSernder.Send(player, null, TopicType.PlayerCreatedTopic);
                return _mapper.Map<PlayerEntity, PlayerDto>(player);
            }
            catch (Exception ex)
            {
                _logger.LogError("Player not created.", ex);
                throw;
            }
        }

        private static PlayerEntity MappingPlayerEntity(PlayerCreateDefaultCommand request) 
        {
            var Player = new PlayerEntity()
            {
                UserId = request.UserId,
                Id = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.Now,
            };

            Player.Infos.Add(
                new PlayerInfoEntity
                {
                    FullName = request.PlayerCreate.FullName,
                    NickName = request.PlayerCreate.NickName,
                    IsActive = true,
                    TeamId = request.TeamId,
                    UpdatedAt = DateTime.Now,
                    Id = Guid.NewGuid().ToString(),
                }
                );
            return Player;
        }
    }
}
