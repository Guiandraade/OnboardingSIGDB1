using OnboardingSIGDB1.Domain.Interfaces.Contexts;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace OnboardingSIGDB1.Domain.Services.Base;

public class BaseService
{
    private readonly INotificationContext _notificationContext;
    
    protected BaseService(INotificationContext notificationContext)
    {
        _notificationContext = notificationContext;
    }
    
    protected T? NotifyError<T>(string key, string message) where T : class
    {
        _notificationContext.AddNotification(key, message);
        return null;
    }

    protected T? AddDomainNotifications<T>(ValidationResult validationResult) where T : class
    {
        _notificationContext.AddRange(validationResult.Errors);
        return null;
    }

    protected bool NotifyErrorBool(string key, string message)
    {
        _notificationContext.AddNotification(key, message);
        return false;
    }

    protected void AddValidationErrors(ValidationResult validationResult)
    {
        _notificationContext.AddRange(validationResult.Errors);
    }

    protected bool AddDomainNotificationsBool(ValidationResult validationResult)
    {
        _notificationContext.AddRange(validationResult.Errors);
        return false;
    }
}