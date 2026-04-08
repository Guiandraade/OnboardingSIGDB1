using System.ComponentModel.DataAnnotations;
using FluentValidation;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;
using OnboardingSIGDB1.Domain.Notifications;

namespace OnboardingSIGDB1.Domain.Base;

public abstract class BaseEntity<T> : BaseElement<T> where T : BaseEntity<T>
{
    public int Id { get; protected set; }
}