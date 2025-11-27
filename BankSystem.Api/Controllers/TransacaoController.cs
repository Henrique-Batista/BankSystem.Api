using BankSystem.Application.DTOs;
using BankSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Api.Controllers;

public sealed class TransacaoController : ControllerBase
{
    private readonly TransacaoService _transacaoService;

    [HttpGet("transacoes", Name = "GetAllTransactions")]
    public async Task<IResult> GetAllTransactionsAsync()
    {
        var transacoes = await _transacaoService.GetAllTransacoesAsync();
        return Results.Ok((object?)transacoes);
    }

    [HttpPost("transacoes", Name = "CreateNewTransaction")]
    public async Task<IResult> CreateNewTransactionAsync([FromBody] TransacaoInputModel transacaoDto)
    {
        if (ModelState.IsValid || transacaoDto == null) return Results.BadRequest("Transacao invalida.");

        var result = await _transacaoService.AddTransacaoAsync(transacaoDto);
        if (result == null) return Results.BadRequest("Falha ao cadastrar a transacao.");
        
        var transacao = await _transacaoService.GetByIdAsync(result.Value);
        return Results.Ok((object?)transacao);
    }
}