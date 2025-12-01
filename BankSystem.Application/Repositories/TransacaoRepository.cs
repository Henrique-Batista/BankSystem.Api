using BankSystem.Domain.Models;
using BankSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Application.Repositories;

public sealed class TransacaoRepository : Repository<Transacao>, ITransacaoRepository
{
    public TransacaoRepository(BankDbContext dbContext) : base(dbContext)
    {
    }
    
    public override async Task<Transacao?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Transacoes
            .Include(t => t.ContaOrigem)
            .Include(t => t.ContaDestino)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
}