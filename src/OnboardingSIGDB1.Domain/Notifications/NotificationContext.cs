using OnboardingSIGDB1.Domain.Interfaces;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;

namespace OnboardingSIGDB1.Domain.Notifications;

public class NotificationContext : INotificationContext
{
    private readonly List<Notification> _notifications = new();

    public IReadOnlyCollection<Notification> Notifications =>  _notifications.AsReadOnly();
    
    public bool HasNotifications => _notifications.Any();
    
    public void AddNotification(string key, string message)
    {
        if (_notifications.Any(n => n.Key == key && n.Message == message))
            return;
            
        _notifications.Add(new Notification(key, message));
    }
    
    public void AddNotifications(IEnumerable<Notification> notifications)
    {
        foreach (var notification in notifications)
        {
            AddNotification(notification.Key, notification.Message);
        }
    }
}