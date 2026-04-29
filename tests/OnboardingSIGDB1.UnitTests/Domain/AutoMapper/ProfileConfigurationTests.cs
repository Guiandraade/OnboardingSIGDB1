using AutoMapper;

namespace OnboardingSIGDB1.UnitTests.Domain.AutoMapper;

public class ProfileConfigurationTests
{
    [Fact]
    public void AllProfilesAreValid()
    {
        var cfg = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new OnboardingSIGDB1.Domain.AutoMapper.CompanyProfile());
            cfg.AddProfile(new OnboardingSIGDB1.Domain.AutoMapper.EmployeeProfile());
            cfg.AddProfile(new OnboardingSIGDB1.Domain.AutoMapper.PositionProfile());
        });

        cfg.AssertConfigurationIsValid();
    }
}