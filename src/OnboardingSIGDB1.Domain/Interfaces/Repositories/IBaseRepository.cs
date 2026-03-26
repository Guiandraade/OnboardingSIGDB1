namespace OnboardingSIGDB1.Domain.Interfaces.Repositories;

public interface IBaseRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<int> SaveChangesAsync();
}