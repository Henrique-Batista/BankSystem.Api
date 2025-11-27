using BankSystem.Domain.Models;
using BankSystem.Infrastructure.Data;

namespace BankSystem.Application.Repositories;

public sealed class TransacaoRepository : Repository<Transacao>, ITransacaoRepository
{
    public TransacaoRepository(BankDbContext dbContext) : base(dbContext)
    {
    }
}