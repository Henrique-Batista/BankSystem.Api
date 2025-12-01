using BankSystem.Application.DTOs;
using BankSystem.Application.Repositories;
using BankSystem.Domain.Models;
using BankSystem.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace BankSystem.Application.Services;

public sealed class TransacaoService : ITransacaoService
{
    private readonly IRepository<Transacao> _transacaoRepository;
    private readonly IContaService _contaService;
    private readonly ILogger<TransacaoService> _logger;

    public TransacaoService(IRepository<Transacao> transacaoRepository, IContaService contaService, ILogger<TransacaoService> logger)
    {
        _transacaoRepository = transacaoRepository;
        _contaService = contaService;
        _logger = logger;
    }

    public async Task<IEnumerable<TransacaoViewModel>> GetAllTransacoesAsync()
    {
        _logger.LogInformation("BankSystem.Api: Buscando todas as transacoes");
        return await _transacaoRepository.GetAllAsync()
            .ContinueWith(task => task.Result.Select(TransacaoToDto));
    }

    public async Task<TransacaoViewModel?> GetByIdAsync(Guid id)
    {
        var transacao = await _transacaoRepository.GetByIdAsync(id);
        if (transacao == null) throw new ArgumentNullException(nameof(transacao));
        _logger.LogInformation("BankSystem.Api: Buscando transacao por id: {id}", id);
        return TransacaoToDto(transacao);
    }
    
    public async Task<Guid?> AddTransacaoAsync(TransacaoInputModel transacaoDto)
    {
        var contaOrigem = await _contaService.GetByIdAsync(transacaoDto.ContaOrigemId);
        var contaDestino = await _contaService.GetByIdAsync(transacaoDto.ContaDestinoId);
        
        if (contaOrigem == null || contaDestino == null) throw new InvalidOperationException("Conta de origem ou destino inexistente.");
        
        if (contaOrigem.Saldo < transacaoDto.Valor) throw new InvalidOperationException("Saldo insuficiente.");
        
        if (contaOrigem.Tipo == nameof(TipoDeConta.Digital) || contaDestino.Tipo == nameof(TipoDeConta.Digital))
        {
            if (transacaoDto.Tipo == TipoDeTransacao.Cheque) throw new InvalidOperationException("Tipo de transacao invalido para contas digitais.");
        }

        await _contaService.Transfer(contaOrigem.Id, contaDestino.Id, transacaoDto.Valor);
        
        _logger.LogInformation("BankSystem.Api: Adicionando nova transacao do tipo {tipo} para contas {contaOrigemId} e {contaDestinoId} com valor {valor}.", transacaoDto.Tipo, transacaoDto.ContaOrigemId, transacaoDto.ContaDestinoId, transacaoDto.Valor);
        return await _transacaoRepository.AddAsync(new Transacao
        (
            transacaoDto.Tipo,
            transacaoDto.Valor,
            transacaoDto.ContaOrigemId,
            transacaoDto.ContaDestinoId
        ));
    }

    public TransacaoViewModel TransacaoToDto(Transacao transacao)
    {
        ArgumentNullException.ThrowIfNull(transacao);
        return new TransacaoViewModel(transacao);
    }
}