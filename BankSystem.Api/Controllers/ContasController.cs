using BankSystem.Application.DTOs;
using BankSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class ContasController : ControllerBase
{
    private readonly IContaService _contaService;

    public ContasController(IContaService contaService)
    {
        _contaService = contaService;
    }

    [EndpointSummary("Get all accounts")]
    [ProducesResponseType(typeof(IEnumerable<ContaViewModel>), StatusCodes.Status200OK)]
    [HttpGet("contas", Name = "GetAllAccounts")]
    public async Task<IResult> GetAllAccountsAsync()
    {
        var contas = await _contaService.GetAllContasAsync();
        return Results.Ok(contas);
    }

    [EndpointSummary("Get account by Id")]
    [ProducesResponseType(typeof(ContaViewModel), StatusCodes.Status200OK)]
    [HttpGet("contas/{id:guid}", Name = "GetAccountById")]
    public async Task<IResult> GetAccountByIdAsync([FromRoute] Guid id)
    {
        var conta = await _contaService.GetByIdAsync(id);
        if (conta == null) return Results.NotFound("Nao foi possivel encontrar a conta com o Id informado.");
        return Results.Ok(conta);
    }

    [EndpointSummary("Get account transactions")]
    [ProducesResponseType(typeof(IEnumerable<TransacaoViewModel>), StatusCodes.Status200OK)]
    [HttpGet("contas/{id:guid}/transacoes", Name = "GetAccountTransactions")]
    public async Task<IResult> GetAccountTransactionsAsync([FromRoute] Guid id)
    {
        var transacoes = await _contaService.GetAccountTransactionsAsync(id);
        return Results.Ok(transacoes);
    }
    
    [EndpointSummary("Get account transactions as destination")]
    [ProducesResponseType(typeof(IEnumerable<TransacaoViewModel>), StatusCodes.Status200OK)]
    [HttpGet("contas/{id:guid}/transacoes/destino", Name = "GetAccountTransactionsAsDestination")]
    public async Task<IResult> GetAccountTransactionsAsDestinationAsync([FromRoute] Guid id)
    {
        var transacoes = await _contaService.GetAccountTransactionsAsDstAsync(id);
        return Results.Ok(transacoes);
    }
    
    [EndpointSummary("Get account transactions as source")]
    [ProducesResponseType(typeof(IEnumerable<TransacaoViewModel>), StatusCodes.Status200OK)]
    [HttpGet("contas/{id:guid}/transacoes/origem", Name = "GetAccountTransactionsAsSource")]
    public async Task<IResult> GetAccountTransactionsAsSourceAsync([FromRoute] Guid id)
    {
        var transacoes = await _contaService.GetAccountTransactionsAsSrcAsync(id);
        return Results.Ok(transacoes);
    }

    [EndpointSummary("Create new account")]
    [ProducesResponseType(typeof(ContaViewModel), StatusCodes.Status201Created)]
    [HttpPost("contas", Name = "CreateNewAccount")]
    public async Task<IResult> CreateNewAccountAsync([FromBody] ContaInputModel? contaDto)
    {
        if (!ModelState.IsValid || contaDto == null) return Results.BadRequest("Conta invalida.");

        var result = await _contaService.AddContaAsync(contaDto);
        if (result == null) return Results.BadRequest("Falha ao cadastrar a conta.");
        
        var conta = await _contaService.GetByIdAsync(result.Value);
        
        return Results.CreatedAtRoute("GetAccountById", new { id = result.Value }, conta);
    }

    [EndpointSummary("Delete account by Id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpDelete("contas/{id:guid}", Name = "DeleteAccount")]
    public async Task<IResult> DeleteAccountAsync([FromRoute] Guid id)
    {
        var result = await _contaService.DeleteAsync(id);
        if (!result) return Results.BadRequest("Falha ao deletar os dados.");
        
        return Results.Ok("Dados deletados com sucesso.");
    }
}