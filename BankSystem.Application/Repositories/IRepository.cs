using BankSystem.Domain.Models;

namespace BankSystem.Application.Repositories;

public interface IRepository<T> where T : Entity
{
    public Task<IEnumerable<T>> GetAllAsync();
    public Task<T?> GetByIdAsync(Guid id);
    public Task<Guid?> AddAsync(T entity);
    public Task<bool> UpdateAsync(T entity);
    public Task<bool> DeleteAsync(Guid id);
}