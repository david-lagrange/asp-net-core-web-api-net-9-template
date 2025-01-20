using AutoMapper;
using CompanyEmployees.Core.Domain.Entities;
using Shared.DataTransferObjects;

namespace CompanyEmployees;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Company, CompanyDto>()
            .ForMember(c => c.FullAddress,
                opt => opt.MapFrom(x => $"{x.Address} {x.Country}"));
        CreateMap<CompanyForCreationDto, Company>();

        CreateMap<Employee, EmployeeDto>();
        CreateMap<EmployeeForCreationDto, Employee>();
    }
}
