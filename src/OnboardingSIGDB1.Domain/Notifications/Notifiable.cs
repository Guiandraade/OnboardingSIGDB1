namespace OnboardingSIGDB1.Domain.Notifications;

public abstract class Notifiable
{
    private readonly List<Notification> _notifications = new();
    
    public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();
    public bool IsValid => !_notifications.Any();

    protected void AddNotification(string key, string message)
    {
        if(_notifications.Any(n => n.Key == key && n.Message == message))
            return;
        
        _notifications.Add(new Notification(key, message));
    }
    
    protected void AddNotification(Notification notification)
    {
        if(_notifications.Any(n => n.Key == notification.Key && n.Message == notification.Message))
            return;
        
        _notifications.Add(notification);
    }
    
    protected void AddNotifications(IEnumerable<Notification> notifications)
    {
        foreach (var notification in notifications)
            AddNotification(notification);
    }

    public void ClearNotifications()
    {
        _notifications.Clear();
    }

    public bool HasNotifications(string key)
    {
        return _notifications.Any(n => n.Key == key);
    }
    
}