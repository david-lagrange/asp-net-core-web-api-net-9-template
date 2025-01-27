using AutoMapper;
using NetCoreWebAPIJWTAuth.Core.Domain.Entities;
using Shared.DataTransferObjects;

namespace NetCoreWebAPIJWTAuth;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserForRegistrationDto, User>();

        CreateMap<BaseEntity, BaseEntityDto>()
            .ForMember(c => c.FullAddress,
                opt => opt.MapFrom(x => $"{x.Address} {x.Country}"));
        CreateMap<BaseEntityForCreationDto, BaseEntity>();
        CreateMap<BaseEntityForUpdateDto, BaseEntity>();

        CreateMap<DependantEntity, DependantEntityDto>();
        CreateMap<DependantEntityForCreationDto, DependantEntity>();
        CreateMap<DependantEntityForUpdateDto, DependantEntity>().ReverseMap();
    }
}
