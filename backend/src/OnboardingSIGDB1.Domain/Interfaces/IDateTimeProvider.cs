namespace OnboardingSIGDB1.Domain.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
