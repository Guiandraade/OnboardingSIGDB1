namespace OnboardingSIGDB1.Domain.Notifications;

public class Notification
{
    public string Key { get; set; }
    public string Message { get; set; }

    public Notification(string key, string message)
    {
        Key = key ?? string.Empty;
        Message = message ?? string.Empty;
    }
}