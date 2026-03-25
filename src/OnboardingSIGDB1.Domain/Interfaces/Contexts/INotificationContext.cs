using OnboardingSIGDB1.Domain.Notifications;

namespace OnboardingSIGDB1.Domain.Interfaces.Contexts;

public interface INotificationContext
{
    IReadOnlyCollection<Notification> Notifications { get; }
    bool HasNotifications { get; }
    void AddNotification(string key, string message);
    void AddNotifications(IEnumerable<Notification> notifications);
}