using FluentValidation.Results;
using OnboardingSIGDB1.Domain.Notifications;

namespace OnboardingSIGDB1.Domain.Interfaces.Contexts;

public interface INotificationContext
{
    IReadOnlyCollection<Notification> Notifications { get; }
    bool IsValid { get; }
    void AddNotification(string key, string message);
    void AddRange(IEnumerable<Notification> notifications);
    void AddRange(IEnumerable<ValidationFailure> failures);
    void Clear();
}