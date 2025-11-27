using BankSystem.Domain.Models;
using BankSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Application.Repositories;

public abstract class Repository<T> : IRepository<T> where T : Entity
{
    protected readonly BankDbContext _dbContext;

    public Repository(BankDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        var entities = await _dbContext.Set<T>().AsNoTracking().ToListAsync();
        return entities;
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Guid?> AddAsync(T? entity)
    {
        if (entity == null) return await Task.FromResult<Guid?>(null);
        
        var test = await _dbContext.Set<T>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return await Task.FromResult(test.Entity.Id);
    }

    public async Task<bool> UpdateAsync(T? entity)
    {
        if (entity == null) return await Task.FromResult(false);
        
        _dbContext.Set<T>().Update(entity);
        await _dbContext.SaveChangesAsync();
        return await Task.FromResult(true);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _dbContext.Set<T>().FindAsync(id);
        if (entity == null) return await Task.FromResult(false);
        
        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync();
        return await Task.FromResult(true);
    }
}