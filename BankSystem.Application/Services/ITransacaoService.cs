using BankSystem.Application.DTOs;
using BankSystem.Domain.Models;

namespace BankSystem.Application.Services;

public interface ITransacaoService
{
    Task<IEnumerable<TransacaoViewModel>> GetAllTransacoesAsync();
    Task<TransacaoViewModel?> GetByIdAsync(Guid id);
    Task<Guid?> AddTransacaoAsync(TransacaoInputModel transacaoDto);
    Task<bool> DeleteAsync(Guid id);
    TransacaoViewModel TransacaoToDto(Transacao transacao);
}