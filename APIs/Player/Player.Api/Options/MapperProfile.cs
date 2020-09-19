using AutoMapper;
using Player.Data.Models.Entites;
using Player.Domains.Models;

namespace Player.Api.Options
{
    public class MapperProfile : Profile
    {
        public MapperProfile()  
        {
            CreateMap<PlayerEntity, PlayerDto>()
                .ForMember(fm => fm.IsActive, op=>op.MapFrom(m=>m.LastInfo().IsActive))
                .ForMember(fm => fm.NickName, op => op.MapFrom(m => m.LastInfo().NickName))
                .ForMember(fm => fm.FullName, op => op.MapFrom(m => m.LastInfo().FullName))
                .ForMember(fm => fm.TeamId, op => op.MapFrom(m => m.LastInfo().TeamId))
                .ForMember(fm => fm.UpdatedAt, op => op.MapFrom(m => m.LastInfo().UpdatedAt))
                .ForMember(fm => fm.CreatedAt, op => op.MapFrom(m => m.CreatedAt))
                .ForMember(fm => fm.Id, op => op.MapFrom(m => m.Id))
                ;
        }
    }
}
