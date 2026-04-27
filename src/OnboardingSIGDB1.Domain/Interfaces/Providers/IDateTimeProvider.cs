namespace OnboardingSIGDB1.Domain.Interfaces.Providers;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}

