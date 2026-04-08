using FluentValidation;

namespace OnboardingSIGDB1.Domain.Base;

public abstract class BaseElement<T> : AbstractValidator<T> where T : class
{
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public abstract bool Validation();
}