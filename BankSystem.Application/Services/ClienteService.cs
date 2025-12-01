using BankSystem.Application.DTOs;
using BankSystem.Application.Repositories;
using BankSystem.Domain.Models;
using Microsoft.Extensions.Logging;

namespace BankSystem.Application.Services;

public sealed class ClienteService : IClienteService
{
    private readonly IRepository<Cliente> _clienteRepository;
    private readonly ILogger<ClienteService> _logger;

    public ClienteService(IRepository<Cliente> clienteRepository, ILogger<ClienteService> logger)
    {
        _clienteRepository = clienteRepository;
        _logger = logger;
    }
    
    public async Task<IEnumerable<ClienteViewModel>> GetAllClientsAsync()
    {
        var clientes = await _clienteRepository.GetAllAsync();
        _logger.LogInformation("BankSystem.Api: Buscando todos os clientes");
        return clientes.Select(ClientToDto);
    }
    
    public async Task<ClienteViewModel?> GetByIdAsync(Guid id)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        ArgumentNullException.ThrowIfNull(cliente);
        _logger.LogInformation("BankSystem.Api: Buscando cliente por id: {id}", id);
        return ClientToDto(cliente);
    }

    public async Task<bool> ChangeClientNameAsync(Guid id, string nome)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        ArgumentNullException.ThrowIfNull(cliente);
        if (cliente.Nome == nome) return false;
        _logger.LogInformation("BankSystem.Api: Alterando nome do cliente; Novo nome:{nome}; Antigo Nome: {antigoNome}", nome, cliente.Nome);
        cliente.Nome = nome;
        return await _clienteRepository.UpdateAsync(cliente);
    } 

    public async Task<Guid?> AddClientAsync(ClienteInputModel clienteDto)
    {
        ArgumentNullException.ThrowIfNull(clienteDto);
        
        _logger.LogInformation("BankSystem.Api: Criando novo cliente: {nome}", clienteDto.Nome);
        return await _clienteRepository.AddAsync(new Cliente
        (
            clienteDto.Nome,
            clienteDto.Cpf,
            DateOnly.Parse(clienteDto.DataNascimento)
        ));
    }
    
    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("BankSystem.Api: Deletando cliente por id: {id}", id);
        return await _clienteRepository.DeleteAsync(id);
    }

    public ClienteViewModel ClientToDto(Cliente cliente)
    {
        ArgumentNullException.ThrowIfNull(cliente);
        return new ClienteViewModel(cliente);
    }

    public async Task<IEnumerable<ContaViewModel>> GetClientAccounts(Guid clienteId)
    {
        var cliente = await _clienteRepository.GetByIdAsync(clienteId);
        _logger.LogInformation("BankSystem.Api: Buscando contas do cliente por id: {id}", clienteId);
        if (cliente == null) return Enumerable.Empty<ContaViewModel>();
        return cliente.Contas.Select(c => new ContaViewModel(c));
    }
}