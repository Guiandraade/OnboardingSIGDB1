using AutoMapper;
using OnboardingSIGDB1.Domain.Dto.Companies.Request;
using OnboardingSIGDB1.Domain.Dto.Companies.Response;
using OnboardingSIGDB1.Domain.Entities.Companies;
using OnboardingSIGDB1.Domain.Entities.Employees;

namespace OnboardingSIGDB1.Domain.AutoMapper;

public class CompanyProfile : Profile
{
    public CompanyProfile()
    {
        MapCompanyRequestToEntity();
        MapCompanyToResponse();
        MapCompanyWithEmployees();
        MapEmployeeToCompanyDetails();
    }
    
    private void MapCompanyRequestToEntity()
    {
        CreateMap<CompanyRequest, Company>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ValidationResult, opt => opt.Ignore())
            .ForMember(dest => dest.Notifications, opt => opt.Ignore());
    }
    
    private void MapCompanyToResponse()
    {
        CreateMap<Company, CompanyResponse>();
    }
    
    private void MapCompanyWithEmployees()
    {
        CreateMap<Company, CompanyAndEmployeesResponse>()
            .ForMember(dest => dest.EmployeesPositionHistory,
                opt => opt.MapFrom(src => src.Employees));
    }
    
    private void MapEmployeeToCompanyDetails()
    {
        CreateMap<Employee, CompanyDetailsResponse>()
            .ForMember(dest => dest.EmployeeId,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.PositionName,
                opt => opt.MapFrom(src => src.CurrentPositionDescription))
            .ForMember(dest => dest.EmployeeName,
                opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.HiringDate,
                opt => opt.MapFrom(src => src.HireDate));
    }
    
}