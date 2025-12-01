using BankSystem.Application.DTOs;

namespace BankSystem.Application.Services;

public interface IContaService
{
    Task<IEnumerable<ContaViewModel>> GetAllContasAsync();
    Task<ContaViewModel?> GetByIdAsync(Guid id);
    Task<Guid?> AddContaAsync(ContaInputModel contaDto);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<TransacaoViewModel>> GetAccountTransactionsAsync(Guid contaId);
    Task<IEnumerable<TransacaoViewModel>> GetAccountTransactionsAsSrcAsync(Guid contaId);
    Task<IEnumerable<TransacaoViewModel>> GetAccountTransactionsAsDstAsync(Guid contaId);
}