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
        if (cliente == null) throw new ArgumentNullException(nameof(cliente));
        
        return ClientToDto(cliente);
    }

    public async Task<Guid?> AddClientAsync(ClienteInputModel clienteDto)
    {
        if (clienteDto == null) throw new ArgumentNullException(nameof(clienteDto));
        
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
        if (cliente == null) throw new ArgumentNullException(nameof(cliente));
        
        return new ClienteViewModel(cliente);
    }

    public async Task<IEnumerable<ContaViewModel>> GetClientAccounts(Guid clienteId)
    {
        var cliente = await _clienteRepository.GetByIdAsync(clienteId);
        if (cliente == null) return Enumerable.Empty<ContaViewModel>();
        return cliente.Contas.Select(c => new ContaViewModel(c));
    }
}