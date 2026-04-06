using AutoMapper;
using OnboardingSIGDB1.Domain.Dto.Positions.Request;
using OnboardingSIGDB1.Domain.Dto.Positions.Response;
using OnboardingSIGDB1.Domain.Entities.Positions;

namespace OnboardingSIGDB1.Domain.AutoMapper;

public class PositionProfile : Profile
{
    public PositionProfile()
    {
        MapPositionRequest();
        MapPositionResponse();
    }

    private void MapPositionRequest()
    {
        CreateMap<PositionRequest, Position>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ValidationResult, opt => opt.Ignore())
            .ForMember(dest => dest.Notifications, opt => opt.Ignore())
            .ForMember(dest => dest.EmployeePositions, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()); // opcional
    }

    private void MapPositionResponse()
    {
        CreateMap<Position, PositionResponse>();
    }
}