using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnboardingSIGDB1.Data.Context;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;
using OnboardingSIGDB1.Domain.Interfaces.Persistence;

namespace OnboardingSIGDB1.Data.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly OnboardingDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly INotificationContext _notificationContext;

    public UnitOfWork(OnboardingDbContext context, ILogger<UnitOfWork> logger, INotificationContext notificationContext)
    {
        _context = context;
        _logger = logger;
        _notificationContext = notificationContext;
    }

    public async Task<bool> CommitAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency conflict detected while committing changes.");
            throw;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while committing changes.");
            _notificationContext.AddNotification("Commit", "Unable to save changes.");
            return false;
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}