using FluentValidation;
using OnboardingSIGDB1.Domain.Notifications;

namespace OnboardingSIGDB1.Domain.Base;

public abstract class BaseElement<T> : AbstractValidator<T> where T : class
{
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    
    private readonly List<Notification> _notifications = new();
    public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();
    
    public bool IsValid => !_notifications.Any();
    
    public abstract bool Validation();
    
    protected void AddNotification(string key, string message)
    {
        if (!_notifications.Any(n => n.Key == key && n.Message == message))
            _notifications.Add(new Notification(key, message));
    }
    
    protected void ClearNotifications() => _notifications.Clear();
}