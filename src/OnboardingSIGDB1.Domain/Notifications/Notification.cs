namespace OnboardingSIGDB1.Domain.Notifications;

/// <summary>
/// Notification entry used to represent validation and business errors.
/// </summary>
public class Notification
{
    /// <summary>
    /// Error key or field name.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Error message.
    /// </summary>
    public string Message { get; set; }

    public Notification(string? key, string? message)
    {
        Key = key ?? string.Empty;
        Message = message ?? string.Empty;
    }
}