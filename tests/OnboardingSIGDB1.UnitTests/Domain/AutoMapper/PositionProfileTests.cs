using AutoMapper;
using FluentAssertions;
using OnboardingSIGDB1.Domain.AutoMapper;
using OnboardingSIGDB1.Domain.Dto.Positions.Request;
using OnboardingSIGDB1.Domain.Dto.Positions.Response;
using OnboardingSIGDB1.Domain.Entities.Positions;

namespace OnboardingSIGDB1.UnitTests.Domain.AutoMapper;

public class PositionProfileTests : MapperTestBase
{
    private readonly IMapper _mapper;

    public PositionProfileTests()
    {
        _mapper = CreateMapper(new PositionProfile());
    }
    
    [Fact]
    public void PositionRequest_To_Position_MapsDescription()
    {
        var request = new PositionRequest("Developer");

        var entity = _mapper.Map<Position>(request);

        entity.Should().NotBeNull();
        entity.Description.Should().Be("Developer");
    }

    [Fact]
    public void PositionRequest_To_Position_IgnoresIdValidationResultEmployeePositions()
    {
        var request = new PositionRequest("Tester");

        var entity = _mapper.Map<Position>(request);

        entity.Id.Should().Be(0);
        entity.ValidationResult.Should().BeNull();
        entity.EmployeePositions.Should().BeEmpty();
    }

    [Fact]
    public void Position_To_PositionResponse_MapsAllProperties()
    {
        var entity = new Position("Architect");

        var dto = _mapper.Map<PositionResponse>(entity);

        dto.Should().NotBeNull();
        dto.Id.Should().Be(0);
        dto.Description.Should().Be("Architect");
    }

    [Fact]
    public void PositionCollection_MapsTo_PositionResponseCollection()
    {
        var list = new[]
        {
            new Position("Dev"),
            new Position("QA"),
            new Position("PM")
        };

        var mapped = _mapper.Map<IEnumerable<PositionResponse>>(list).ToList();

        mapped.Should().HaveCount(3);
        mapped[0].Description.Should().Be("Dev");
        mapped[1].Description.Should().Be("QA");
        mapped[2].Description.Should().Be("PM");
    }

    [Fact]
    public void NullPosition_ReturnsNull()
    {
        Position? src = null;
        var dto = _mapper.Map<PositionResponse?>(src);
        dto.Should().BeNull();
    }
}

