using FluentValidation.Results;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;

namespace OnboardingSIGDB1.Domain.Notifications;

public sealed class NotificationContext : INotificationContext
{
    private readonly List<Notification> _notifications = new();
    public IReadOnlyCollection<Notification> Notifications =>  _notifications.AsReadOnly();
    public bool IsValid => !_notifications.Any();
    
    public void AddNotification(string key, string message)
    {
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(message))
            return;
        
        if (_notifications.Any(n => n.Key == key && n.Message == message))
            return;
            
        _notifications.Add(new Notification(key, message));
    }
    
    public void AddRange(IEnumerable<Notification> notifications)
    {
        if (notifications == null) return;
        
        foreach (var notification in notifications.Where(n => n != null))
            AddNotification(notification.Key, notification.Message);
    }
    
    public void AddRange(IEnumerable<ValidationFailure> failures)
    {
        if (failures == null)
            return;

        foreach (var f in failures)
            AddNotification(f.PropertyName, f.ErrorMessage);
    }
    public void Clear()
    {
        _notifications.Clear();
    }
}