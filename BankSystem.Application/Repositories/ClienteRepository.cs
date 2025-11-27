using BankSystem.Domain.Models;
using BankSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BankSystem.Application.Repositories;

public sealed class ClienteRepository : Repository<Cliente>, IClienteRepository
{
    private readonly ILogger<ClienteRepository> _logger;

    public ClienteRepository(BankDbContext dbContext, ILogger<ClienteRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }
    
    public override async Task<Cliente?> GetByIdAsync(Guid id)
    {
        var cliente = await _dbContext.Clientes
            .Include(c => c.Contas)
            .FirstOrDefaultAsync(c => c.Id == id);
        return cliente;
    }
}