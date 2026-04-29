using AutoMapper;

namespace OnboardingSIGDB1.UnitTests.Domain.AutoMapper;

public abstract class MapperTestBase
{
    protected IMapper CreateMapper(params Profile[] profiles)
    {
        var cfg = new MapperConfiguration(x =>
        {
            foreach (var p in profiles)
                x.AddProfile(p);
        });

        cfg.AssertConfigurationIsValid();

        return cfg.CreateMapper();
    }
}