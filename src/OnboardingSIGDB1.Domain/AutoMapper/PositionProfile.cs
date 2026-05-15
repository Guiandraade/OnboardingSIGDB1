using AutoMapper;
using OnboardingSIGDB1.Domain.Dto.Positions.Commands;
using OnboardingSIGDB1.Domain.Dto.Positions.Responses;
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
            .ForMember(dest => dest.EmployeePositions, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // opcional
            // AbstractValidator<T> members inherited via BaseElement<T>
            .ForMember("ClassLevelCascadeMode", opt => opt.Ignore())
            .ForMember("RuleLevelCascadeMode", opt => opt.Ignore());
    }

    private void MapPositionResponse()
    {
        CreateMap<Position, PositionResponse>();
    }
}