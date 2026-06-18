using Microsoft.AspNetCore.Mvc;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;

namespace OnboardingSIGDB1.API.Controllers;

public abstract class ApiBaseController : ControllerBase
{
    protected void AddModelStateNotifications(INotificationContext notificationContext)
    {
        foreach (var item in ModelState)
        {
            var key = string.IsNullOrWhiteSpace(item.Key) ? "Model" : item.Key;
            foreach (var error in item.Value.Errors)
            {
                notificationContext.AddNotification(key, error.ErrorMessage);
            }
        }
    }

    protected IActionResult NotificationError(INotificationContext notificationContext)
    {
        var notifications = notificationContext.Notifications;

        // Business "not found" cases should be explicit 404 instead of generic 400.
        if (notifications.Any(n =>
                n.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)))
        {
            return NotFound(notifications);
        }

        // Persistence failures are infrastructure failures and should not be exposed as 400.
        if (notifications.Any(n =>
                n.Key.Equals("Commit", StringComparison.OrdinalIgnoreCase)))
        {
            return StatusCode(StatusCodes.Status500InternalServerError, notifications);
        }

        return BadRequest(notifications);
    }
}
