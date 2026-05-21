using FluentValidation;

namespace OnboardingSIGDB1.Domain.Base;

public abstract class BaseElement<T> : AbstractValidator<T> where T : class
{
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    protected bool RulesRegistered { get; private set; }

    protected void MarkRulesAsRegistered() => RulesRegistered = true;

    public abstract bool Validation();
}