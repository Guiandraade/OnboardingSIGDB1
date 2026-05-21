using Microsoft.EntityFrameworkCore;
using OnboardingSIGDB1.Data.Context;
using OnboardingSIGDB1.Domain.Interfaces.Repositories;

namespace OnboardingSIGDB1.Data.Repositories;

public class BaseRepository<T>(OnboardingDbContext context) : IBaseRepository<T>
    where T : class
{
    protected readonly OnboardingDbContext Context = context;
    protected readonly DbSet<T> DbSet = context.Set<T>();

    public virtual async Task<T?> GetByIdAsync(int id) => await DbSet.FindAsync(id);
    
    public virtual async Task AddAsync(T entity) => await DbSet.AddAsync(entity);
    
    public virtual void Delete(T entity) => DbSet.Remove(entity);
    
}