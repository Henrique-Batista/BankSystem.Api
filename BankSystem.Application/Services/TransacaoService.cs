using BankSystem.Application.DTOs;
using BankSystem.Application.Repositories;
using BankSystem.Domain.Models;
using Microsoft.Extensions.Logging;

namespace BankSystem.Application.Services;

public sealed class TransacaoService : ITransacaoService
{
    private readonly IRepository<Transacao> _transacaoRepository;
    private readonly IRepository<Conta> _contaRepository;

    public TransacaoService(IRepository<Transacao> transacaoRepository, IRepository<Conta> contaRepository)
    {
        _transacaoRepository = transacaoRepository;
        _contaRepository = contaRepository;
    }

    public async Task<IEnumerable<TransacaoViewModel>> GetAllTransacoesAsync()
    {
        return await _transacaoRepository.GetAllAsync()
            .ContinueWith(task => task.Result.Select(TransacaoToDto));
    }

    public async Task<TransacaoViewModel> GetByIdAsync(Guid id)
    {
        var transacao = await _transacaoRepository.GetByIdAsync(id);
        if (transacao == null) throw new ArgumentNullException(nameof(transacao));

        return TransacaoToDto(transacao);
    }
    
    public async Task<Guid?> AddTransacaoAsync(TransacaoInputModel transacaoDto)
    {
        var contaOrigem = await _contaRepository.GetByIdAsync(transacaoDto.ContaOrigemId);
        var contaDestino = await _contaRepository.GetByIdAsync(transacaoDto.ContaDestinoId);
        
        if (contaOrigem == null || contaDestino == null) throw new InvalidOperationException("Conta de origem ou destino inexistente.");
        
        return await _transacaoRepository.AddAsync(new Transacao
        (
            transacaoDto.Tipo,
            transacaoDto.Valor,
            transacaoDto.ContaOrigemId,
            transacaoDto.ContaDestinoId
        ));
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return _transacaoRepository.DeleteAsync(id);
    }

    public TransacaoViewModel TransacaoToDto(Transacao transacao)
    {
        if (transacao == null) throw new ArgumentNullException(nameof(transacao));
        return new TransacaoViewModel(transacao);
    }
}