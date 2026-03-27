using FluentValidation;
using OnboardingSIGDB1.Domain.Notifications;

namespace OnboardingSIGDB1.Domain.Base;

public abstract class BaseEntity<T> : BaseElement<T> where T : class
{
    public int Id { get; protected set; }
}