namespace OnboardingSIGDB1.Domain.Interfaces.Persistence;

public interface IUnitOfWork : IDisposable
{ 
    Task<bool> CommitAsync();
}