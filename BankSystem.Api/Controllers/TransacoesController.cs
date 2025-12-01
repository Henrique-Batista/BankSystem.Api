using BankSystem.Application.DTOs;
using BankSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class TransacoesController : ControllerBase
{
    private readonly ITransacaoService _transacaoService;

    public TransacoesController(ITransacaoService transacaoService)
    {
        _transacaoService = transacaoService;
    }

    [HttpGet("transacoes", Name = "GetAllTransactions")]
    public async Task<IResult> GetAllTransactionsAsync()
    {
        var transacoes = await _transacaoService.GetAllTransacoesAsync();
        return Results.Ok(transacoes);
    }
    
    [HttpGet("transacoes/{id:guid}", Name = "GetTransactionById")]
    public async Task<IResult> GetTransactionByIdAsync([FromRoute] Guid id)
    {
        var transacao = await _transacaoService.GetByIdAsync(id);
        return Results.Ok(transacao);
    }

    [HttpPost("transacoes", Name = "CreateNewTransaction")]
    public async Task<IResult> CreateNewTransactionAsync([FromBody] TransacaoInputModel? transacaoDto)
    {
        if (!ModelState.IsValid || transacaoDto == null) return Results.BadRequest("Transacao invalida.");

        var result = await _transacaoService.AddTransacaoAsync(transacaoDto);
        if (result == null) return Results.BadRequest("Falha ao cadastrar a transacao.");
        
        var transacao = await _transacaoService.GetByIdAsync(result.Value);
        return Results.Ok(transacao);
    }
}