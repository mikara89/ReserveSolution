using AutoMapper;
using System;
using Team.Api.Models;
using Team.Data.Models.Entites;

namespace Team.Api.Options
{
    public class MapperProfile : Profile
    {
        public MapperProfile()  
        {
            CreateMap<TeamEntity, TeamDto>();
            CreateMap<TeamCreate, TeamEntity>()
                 .ForMember(
                fm => fm.Id,
                op => op.MapFrom(m => Guid.NewGuid().ToString()
                )).ForMember(
                fm => fm.IsActive,
                op => op.MapFrom(m => true
                )).ForMember(
                fm => fm.CreatedAt,
                op => op.MapFrom(m => DateTime.Now
                ))
                 ;
            CreateMap<TeamUpdate, TeamEntity>();
        }
    }
}
