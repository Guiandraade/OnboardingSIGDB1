using OnboardingSIGDB1.Domain.Interfaces;

namespace OnboardingSIGDB1.IOC;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
