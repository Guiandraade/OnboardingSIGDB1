using OnboardingSIGDB1.Data.Context;
using OnboardingSIGDB1.Domain.Interfaces.Persistence;

namespace OnboardingSIGDB1.Data.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly OnboardingDbContext _context;
    
    public UnitOfWork(OnboardingDbContext context)
    {
        _context = context;
    }
    
    public async Task<bool> CommitAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
}