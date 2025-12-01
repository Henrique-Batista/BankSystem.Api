using BankSystem.Domain.Models;
using BankSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BankSystem.Application.Repositories;

public sealed class ContaRepository : Repository<Conta>, IContaRepository
{
    private readonly ILogger<ContaRepository> _logger;

    public ContaRepository(BankDbContext dbContext, ILogger<ContaRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }
    
    public override async Task<Conta?> GetByIdAsync(Guid id)
    {
        var conta = await _dbContext.Contas
            .Include(c => c.TransacoesComoDestino)
            .Include(c => c.TransacoesComoOrigem)
            .FirstOrDefaultAsync(t => t.Id == id);
        return conta;
    }
    
    public new async Task<bool> UpdateAsync(Conta? entity)
    {
        if (entity == null) return await Task.FromResult(false);
        _dbContext.Entry(entity).State = EntityState.Modified;
        _dbContext.Entry(entity).Property(x => x.Numero).IsModified = false;
        await _dbContext.SaveChangesAsync();
        return await Task.FromResult(true);
    }
}