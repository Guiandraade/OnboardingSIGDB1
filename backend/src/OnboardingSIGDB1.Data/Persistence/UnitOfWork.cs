using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnboardingSIGDB1.Data.Context;
using OnboardingSIGDB1.Domain.Interfaces.Persistence;

namespace OnboardingSIGDB1.Data.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly OnboardingDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(OnboardingDbContext context, ILogger<UnitOfWork> logger)
    {
        _context = context;
        _logger = logger;
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
            // Optimistic concurrency conflict — let the global middleware handle the HTTP response,
            // but rethrow so the caller is not silently swallowed.
            _logger.LogError(ex, "Concurrency conflict detected while committing changes.");
            throw;
        }
        catch (DbUpdateException ex)
        {
            // Database constraint violation or infrastructure failure (e.g. FK, unique index,
            // connection drop). Returning false integrates cleanly with the notification pattern:
            // the service layer will call NotifyError("Commit", "Unable to save changes.").
            _logger.LogError(ex, "Database error while committing changes.");
            return false;
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}