using BankSystem.Application.DTOs;
using BankSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Api.Controllers;

[ApiController]
[Route("api")]
public sealed class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;
    
    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [EndpointSummary("Get all clients")]
    [HttpGet("clientes", Name = "GetAllClients")]
    [ProducesResponseType(typeof(IEnumerable<ClienteViewModel>), StatusCodes.Status200OK)]
    public async Task<IResult> GetAllClientsAsync()
    {
        var clientes = await _clienteService.GetAllClientsAsync();
        return Results.Ok(clientes);
    }

    [EndpointSummary("Get client by Id")]
    [ProducesResponseType(typeof(ClienteViewModel), StatusCodes.Status200OK)]
    [HttpGet("clientes/{id:guid}", Name = "GetClientById")]
    public async Task<IResult> GetClientByIdAsync([FromRoute] Guid id)
    {
        var cliente = await _clienteService.GetByIdAsync(id);
        if (cliente == null) return Results.NotFound("Nao foi possivel encontrar o cliente com o Id informado.");
        
        return Results.Ok(cliente);
    }

    [EndpointSummary("Create new client")]
    [ProducesResponseType(typeof(ClienteViewModel), StatusCodes.Status201Created)]
    [HttpPost("clientes", Name = "CreateNewClient")]
    public async Task<IResult> CreateNewClientAsync([FromBody] ClienteInputModel? cliente)
    {
        if (!ModelState.IsValid || cliente == null) return Results.BadRequest("Cliente invalido.");

        var result = await _clienteService.AddClientAsync(cliente);
        if (result == null) return Results.BadRequest("Falha ao cadastrar o cliente.");
        
        return Results.CreatedAtRoute("GetClientById", new { id = result.Value },
            new ClienteViewModel(result.Value, cliente.Nome, cliente.Cpf, cliente.DataNascimento));
    }
    
    [EndpointSummary("Change client name")]
    [ProducesResponseType(typeof(ClienteViewModel), StatusCodes.Status200OK)]
    [HttpPatch("clientes/{id:guid}", Name = "ChangeClientName")]
    public async Task<IResult> ChangeClientNameAsync([FromRoute] Guid id, [FromBody] string nome)
    {
        var result = await _clienteService.ChangeClientNameAsync(id, nome);
        if (!result) return Results.BadRequest("Nome informado igual ao nome atual.");
        
        return Results.Ok(await _clienteService.GetByIdAsync(id));
    }

    [EndpointSummary("Delete client by Id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpDelete("clientes/{id:guid}", Name = "DeleteClient")]
    public async Task<IResult> DeleteAsync([FromRoute] Guid id)
    {
        var result = await _clienteService.DeleteAsync(id);
        if (!result) return Results.BadRequest("Falha ao deletar os dados.");
        
        return Results.Ok("Dados deletados com sucesso.");
    }

    [EndpointSummary("Get client accounts")]
    [ProducesResponseType(typeof(IEnumerable<ContaViewModel>), StatusCodes.Status200OK)]
    [HttpGet("clientes/{id:guid}/contas", Name = "GetClientAccounts")]
    public async Task<IResult> GetClientAccountsAsync([FromRoute] Guid id)
    {
        var contas = await _clienteService.GetClientAccounts(id);
        return Results.Ok(contas);
    }
}