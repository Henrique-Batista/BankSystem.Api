using BankSystem.Application.DTOs;
using BankSystem.Application.Repositories;
using BankSystem.Domain.Models;
using BankSystem.Domain.ValueObjects;

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
        ArgumentNullException.ThrowIfNull(conta);

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
    
    public async Task<bool> ActivateAccountAsync(Guid contaId, ClienteInputModel contaDto)
    {
        var conta = await _contaRepository.GetByIdAsync(contaId);
        var client = conta.Cliente;
        ArgumentNullException.ThrowIfNull(conta);
        
        await _contaRepository.UpdateAsync(conta);
        return true;
    }

    public async Task<IEnumerable<TransacaoViewModel>> GetAccountTransactionsAsync(Guid contaId)
    {
        var conta = await _contaRepository.GetByIdAsync(contaId);
        ArgumentNullException.ThrowIfNull(conta);
        
        var transacoes = conta.Transacoes;
        return transacoes.Select(t => new TransacaoViewModel(t));
    }

    public async Task<IEnumerable<TransacaoViewModel>> GetAccountTransactionsAsSrcAsync(Guid contaId)
    {
        var conta = await _contaRepository.GetByIdAsync(contaId);
        ArgumentNullException.ThrowIfNull(conta);
        
        var transacoes = conta.TransacoesComoOrigem;
        return transacoes.Select(t => new TransacaoViewModel(t));
    }

    public async Task<IEnumerable<TransacaoViewModel>> GetAccountTransactionsAsDstAsync(Guid contaId)
    {
        var conta = await _contaRepository.GetByIdAsync(contaId);
        ArgumentNullException.ThrowIfNull(conta);
        
        var transacoes = conta.TransacoesComoDestino;
        return transacoes.Select(t => new TransacaoViewModel(t));
    }

    public async Task Transfer(Guid contaOrigemId, Guid contaDestinoId, decimal valor)
    {
        var contaOrigem = await _contaRepository.GetByIdAsync(contaOrigemId);
        var contaDestino = await _contaRepository.GetByIdAsync(contaDestinoId);
        
        if (contaOrigem == null || contaDestino == null) throw new InvalidOperationException("Conta de origem ou destino inexistente.");
        
        if (contaOrigem.Saldo < valor) throw new InvalidOperationException("Saldo insuficiente.");
        if (contaOrigem.Id == contaDestino.Id) throw new InvalidOperationException("Conta origem e destino iguais.");
        if (valor <= 0) throw new ArgumentOutOfRangeException(nameof(valor), "Valor deve ser maior que zero.");
        
        if (contaOrigem.StatusDaConta != StatusDaConta.Ativo || contaDestino.StatusDaConta != StatusDaConta.Ativo)
            throw new InvalidOperationException("Ambas as contas devem estar ativas para realizar a transferencia.");
        
        // testar se ao a operacao de saque da conta origem ser feita porem dar excecao na operacao de deposito na conta destino ira salvar um estado incorreto na aplicacao
        contaOrigem.Sacar(valor);
        contaDestino.Depositar(valor);
        await _contaRepository.UpdateAsync(contaOrigem);
        await _contaRepository.UpdateAsync(contaDestino);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await  _contaRepository.DeleteAsync(id);
    }

    private ContaViewModel ContaToDto(Conta conta)
    {
        ArgumentNullException.ThrowIfNull(conta);
        return new ContaViewModel(conta);
    }
}