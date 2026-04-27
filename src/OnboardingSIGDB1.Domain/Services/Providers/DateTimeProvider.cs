using OnboardingSIGDB1.Domain.Interfaces.Providers;

namespace OnboardingSIGDB1.Domain.Services.Providers;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}

