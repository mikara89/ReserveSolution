using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
using System;

namespace Player.Service.Hendlers
{
    public class PlayerCreateHandler : IRequestHandler<PlayerCreateCommand, PlayerDto>
    {
        private readonly IMapper _mapper;
        private readonly IPlayerEventEmitter _eventSernder;
        private readonly ILogger<PlayerCreateHandler> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IInvetationRepository  _invetationRepository;

        public PlayerCreateHandler(IMapper mapper,
                                 IPlayerEventEmitter eventSernder,
                                 ILogger<PlayerCreateHandler> logger,
                                 IPlayerRepository playerRepository, 
                                 IInvetationRepository invetationRepository)
        {
            _mapper = mapper;
            _eventSernder = eventSernder;
            _logger = logger;
            _playerRepository = playerRepository;
            _invetationRepository = invetationRepository;
        }

        public async Task<PlayerDto> Handle(PlayerCreateCommand request, CancellationToken cancellationToken)
        {
            var invitation = await ValidatingInvitationCode(request.InvitationId);

            var player = MappingPlayerEntity(request, invitation);

            var playerDto = await SavePlayerAndRetuntDtoAsync(player,invitation.Id);

            await UpdateInvetationCodeAsUsedAsync(invitation);

            return playerDto;
        }

        private async Task UpdateInvetationCodeAsUsedAsync(InvetationEntity invitation)
        {
            invitation.IsUsed = true;
            _invetationRepository.Update(invitation);
            await _invetationRepository.SaveAsync();
        }

        private async Task<PlayerDto> SavePlayerAndRetuntDtoAsync(PlayerEntity player, string invitationId)
        {
            try
            {
                _playerRepository.Create(player);
                await _playerRepository.SaveAsync();
                _logger.LogInformation("Player created.");
                _eventSernder.Send(player, invitationId, TopicType.PlayerCreatedTopic);
                return _mapper.Map<PlayerEntity, PlayerDto>(player);
            }
            catch (Exception ex)
            {
                _logger.LogError("Player not created.", ex);
                throw;
            }
        }

        private static PlayerEntity MappingPlayerEntity(PlayerCreateCommand request, InvetationEntity invitation)
        {
            var Player = new PlayerEntity()
            {
                UserId = request.UserId,
                Id = Guid.NewGuid().ToString(),
                CreatedAt= DateTime.Now,
            };

            Player.Infos.Add(
                new PlayerInfoEntity
                {
                    FullName = request.PlayerCreate.FullName,
                    NickName = request.PlayerCreate.NickName,
                    IsActive = true,
                    TeamId = invitation.TeamId,
                    UpdatedAt = DateTime.Now,
                    Id = Guid.NewGuid().ToString(),
                }
                );
            return Player;
        }

        private async Task<InvetationEntity> ValidatingInvitationCode(string invitationId) 
        {
            var invitation = await _invetationRepository
                            .FindByCondition(invs => invs.Id == invitationId).FirstOrDefaultAsync();

            if (invitation == null || invitation.IsCanceled || invitation.IsUsed || IsExpired(invitation.Expiration))
            {
                _logger.LogWarning("Invalid invitation code.");
                throw new InvalidInvitationCodeException();
            } 
            return invitation;
        }

        private static bool IsExpired(DateTime date)
        {
            return date < DateTime.Now;
        }
    }
}
