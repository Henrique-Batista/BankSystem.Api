using BankSystem.Application.DTOs;
using BankSystem.Domain.Models;

namespace BankSystem.Application.Services;

public interface IClienteService
{
    Task<IEnumerable<ClienteViewModel>> GetAllClientsAsync();
    Task<ClienteViewModel?> GetByIdAsync(Guid id);
    Task<Guid?> AddClientAsync(ClienteInputModel clienteDto);
    Task<bool> DeleteAsync(Guid id);
    ClienteViewModel ClientToDto(Cliente cliente);
    Task<IEnumerable<ContaViewModel>> GetClientAccounts(Guid clienteId);
    Task<bool> ChangeClientNameAsync(Guid id, string nome);
}