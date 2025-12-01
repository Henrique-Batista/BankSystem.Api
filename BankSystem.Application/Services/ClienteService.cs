using BankSystem.Application.DTOs;
using BankSystem.Application.Repositories;
using BankSystem.Domain.Models;

namespace BankSystem.Application.Services;

public sealed class ClienteService : IClienteService
{
    private readonly IRepository<Cliente> _clienteRepository;
    public ClienteService(IRepository<Cliente> clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }
    
    public async Task<IEnumerable<ClienteViewModel>> GetAllClientsAsync()
    {
        var clientes = await _clienteRepository.GetAllAsync();
        return clientes.Select(ClientToDto);
    }
    
    public async Task<ClienteViewModel?> GetByIdAsync(Guid id)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        ArgumentNullException.ThrowIfNull(cliente);
        
        return ClientToDto(cliente);
    }

    public async Task<bool> ChangeClientNameAsync(Guid id, string nome)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        ArgumentNullException.ThrowIfNull(cliente);
        if (cliente.Nome == nome) return false;
        
        cliente.Nome = nome;
        return await _clienteRepository.UpdateAsync(cliente);
    } 

    public async Task<Guid?> AddClientAsync(ClienteInputModel clienteDto)
    {
        ArgumentNullException.ThrowIfNull(clienteDto);

        return await _clienteRepository.AddAsync(new Cliente
        (
            clienteDto.Nome,
            clienteDto.Cpf,
            DateOnly.Parse(clienteDto.DataNascimento)
        ));
    }
    
    public async Task<bool> DeleteAsync(Guid id)
    {
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
        if (cliente == null) return Enumerable.Empty<ContaViewModel>();
        return cliente.Contas.Select(c => new ContaViewModel(c));
    }
}