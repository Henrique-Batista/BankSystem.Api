using BankSystem.Application.DTOs;
using BankSystem.Application.Repositories;
using BankSystem.Domain.Models;

namespace BankSystem.Application.Services;

public sealed class ContaService : IContaService
{
    private readonly IRepository<Conta> _contaRepository;
    private readonly IRepository<Cliente> _clienteRepository;
    public ContaService(IRepository<Conta> contaRepository, IRepository<Cliente> clienteRepository)
    {
        _contaRepository = contaRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task<IEnumerable<ContaViewModel>> GetAllContasAsync()
    {
        var contas = await _contaRepository.GetAllAsync();
        return contas.Select(ContaToDto);
    }

    public async Task<ContaViewModel?> GetByIdAsync(Guid id)
    {
        var conta = await _contaRepository.GetByIdAsync(id);
        if (conta == null) throw new ArgumentNullException(nameof(conta));

        return ContaToDto(conta);
    }

    public async Task<Guid?> AddContaAsync(ContaInputModel contaDto)
    {
        var cliente = await _clienteRepository.GetByIdAsync(contaDto.ClienteId);
        
        if (cliente == null) throw new InvalidOperationException("Nao e possivel criar uma conta para um cliente inexistente.");
        
        return await _contaRepository.AddAsync(new Conta
        (
            contaDto.Tipo,
            cliente.Id
        ));
    }

    public async Task<IEnumerable<TransacaoViewModel>> GetAccountTransactionsAsync(Guid contaId)
    {
        var conta = await _contaRepository.GetByIdAsync(contaId);
        if (conta == null) throw new ArgumentNullException(nameof(conta));
        
        var transacoes = conta.Transacoes;
        return transacoes.Select(t => new TransacaoViewModel(t));
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await  _contaRepository.DeleteAsync(id);
    }

    private ContaViewModel ContaToDto(Conta conta)
    {
        if (conta == null) throw new ArgumentNullException("Conta nao encontrada.");
        return new ContaViewModel(conta);
    }
}