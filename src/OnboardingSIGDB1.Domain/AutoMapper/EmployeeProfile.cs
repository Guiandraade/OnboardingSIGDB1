using AutoMapper;
using OnboardingSIGDB1.Domain.Dto.Employees.Request;
using OnboardingSIGDB1.Domain.Dto.Employees.Response;
using OnboardingSIGDB1.Domain.Entities.Employees;

namespace OnboardingSIGDB1.Domain.AutoMapper;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        MapEmployeePositionHistory();
        MapEmployeeRequest();
        MapEmployeeWithPositions();
        MapEmployeeResponse();
        MapEmployeeUpdateRequest();
    }
    
    private void MapEmployeePositionHistory()
    {
        CreateMap<EmployeePosition, EmployeePositionHistoryResponse>()
            .ForMember(dest => dest.PositionName,
                opt => opt.MapFrom(src => src.Position.Description));
    }

    private void MapEmployeeRequest()
    {
        CreateMap<EmployeeRequest, Employee>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Company, opt => opt.Ignore())
            .ForMember(dest => dest.CompanyId, opt => opt.Ignore())
            .ForMember(dest => dest.Positions, opt => opt.Ignore())
            .ForMember(dest => dest.ValidationResult, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember("ClassLevelCascadeMode", opt => opt.Ignore())
            .ForMember("RuleLevelCascadeMode", opt => opt.Ignore());
    }
    
    
    private void MapEmployeeWithPositions()
    {
        CreateMap<Employee, EmployeeAndPositionsResponse>()
            .ForMember(dest => dest.CompanyName,
                opt => opt.MapFrom(src => src.Company.Name))
            .ForMember(dest => dest.CurrentPosition,
                opt => opt.MapFrom(src => src.CurrentPositionDescription))
            .ForMember(dest => dest.PositionHistory,
                opt => opt.MapFrom(src =>
                    src.Positions.OrderByDescending(p => p.StartDate)));
    }

    private void MapEmployeeResponse()
    {
        CreateMap<Employee, EmployeeResponse>()
            .ForMember(dest => dest.CompanyName,
                opt => opt.MapFrom(src => src.Company.Name))
            .ForMember(dest => dest.CurrentPosition,
                opt => opt.MapFrom(src => src.CurrentPositionDescription));
    }

    private void MapEmployeeUpdateRequest()
    {
        CreateMap<EmployeeUpdateRequest, Employee>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ValidationResult, opt => opt.Ignore())
            .ForMember(dest => dest.CompanyId, opt => opt.Ignore())
            .ForMember(dest => dest.Company, opt => opt.Ignore())
            .ForMember(dest => dest.Positions, opt => opt.Ignore())
            .ForMember(dest => dest.HireDate, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember("ClassLevelCascadeMode", opt => opt.Ignore())
            .ForMember("RuleLevelCascadeMode", opt => opt.Ignore());
    }
}