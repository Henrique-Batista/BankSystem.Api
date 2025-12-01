using BankSystem.Application.DTOs;
using BankSystem.Application.Repositories;
using BankSystem.Domain.Models;
using BankSystem.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace BankSystem.Application.Services;

public sealed class ContaService : IContaService
{
    private readonly IRepository<Conta> _contaRepository;
    private readonly IClienteService _clienteService;
    private readonly ILogger<ContaService> _logger;
    const decimal PorcentagemTaxaTransferencia = 0.10m;

    public ContaService(IRepository<Conta> contaRepository, IClienteService clienteService, ILogger<ContaService> logger)
    {
        _contaRepository = contaRepository;
        _clienteService = clienteService;
        _logger = logger;
    }

    public async Task<IEnumerable<ContaViewModel>> GetAllContasAsync()
    {
        var contas = await _contaRepository.GetAllAsync();
        _logger.LogInformation("BankSystem.Api: Buscando todas as contas");
        return contas.Select(ContaToDto);
    }

    public async Task<ContaViewModel?> GetByIdAsync(Guid id)
    {
        var conta = await _contaRepository.GetByIdAsync(id);
        ArgumentNullException.ThrowIfNull(conta);
        _logger.LogInformation("BankSystem.Api: Buscando conta por id: {id}", id);
        return ContaToDto(conta);
    }

    public async Task<Guid?> AddContaAsync(ContaInputModel contaDto)
    {
        var cliente = await _clienteService.GetByIdAsync(contaDto.ClienteId);
        if (cliente == null)
            throw new InvalidOperationException("Nao e possivel criar uma conta para um cliente inexistente.");
        _logger.LogInformation("BankSystem.Api: Criando nova conta para cliente: {nome}", cliente.Nome);
        return await _contaRepository.AddAsync(new Conta(contaDto.Tipo, cliente.Id));
    }

    public async Task<bool> ActivateAccountAsync(Guid contaId, ClienteInputModel clienteDto)
    {
        var conta = await _contaRepository.GetByIdAsync(contaId);
        ArgumentNullException.ThrowIfNull(conta);
        var client = conta.Cliente;
        if (!(clienteDto.Nome == client?.Nome && clienteDto.Cpf == client.Cpf &&
              clienteDto.DataNascimento == client.DataNascimento.ToString()))
        {
            throw new InvalidOperationException("Dados do cliente nao correspondem aos dados da conta.");
        }

        conta.StatusDaConta = conta.StatusDaConta == StatusDaConta.Inativo
            ? StatusDaConta.Ativo
            : throw new InvalidOperationException("Conta ja esta ativa.");
        await _contaRepository.UpdateAsync(conta);
        _logger.LogInformation("BankSystem.Api: Conta {contaId} ativada com sucesso.", contaId);
        return true;
    }

    public async Task<bool> DesactivateAccountAsync(Guid contaId, ClienteInputModel clienteDto)
    {
        var conta = await _contaRepository.GetByIdAsync(contaId);
        ArgumentNullException.ThrowIfNull(conta);
        var client = conta.Cliente;
        if (!(clienteDto.Nome == client?.Nome && clienteDto.Cpf == client.Cpf &&
              clienteDto.DataNascimento == client.DataNascimento.ToString()))
        {
            throw new InvalidOperationException("Dados do cliente nao correspondem aos dados da conta.");
        }

        conta.StatusDaConta = conta.StatusDaConta == StatusDaConta.Ativo
            ? StatusDaConta.Inativo
            : throw new InvalidOperationException("Conta ja esta desativada.");
        await _contaRepository.UpdateAsync(conta);
        _logger.LogInformation("BankSystem.Api: Conta {contaId} desativada com sucesso.", contaId);
        return true;
    }

    public async Task<IEnumerable<TransacaoViewModel>> GetAccountTransactionsAsync(Guid contaId)
    {
        var conta = await _contaRepository.GetByIdAsync(contaId);
        ArgumentNullException.ThrowIfNull(conta);
        var transacoes = conta.Transacoes;
        _logger.LogInformation("BankSystem.Api: Buscando transacoes da conta por id: {id}", contaId);
        return transacoes.Select(t => new TransacaoViewModel(t));
    }

    public async Task<IEnumerable<TransacaoViewModel>> GetAccountTransactionsAsSrcAsync(Guid contaId)
    {
        var conta = await _contaRepository.GetByIdAsync(contaId);
        ArgumentNullException.ThrowIfNull(conta);
        var transacoes = conta.TransacoesComoOrigem;
        _logger.LogInformation("BankSystem.Api: Buscando transacoes da conta como origem por id: {id}", contaId);
        return transacoes.Select(t => new TransacaoViewModel(t));
    }

    public async Task<IEnumerable<TransacaoViewModel>> GetAccountTransactionsAsDstAsync(Guid contaId)
    {
        var conta = await _contaRepository.GetByIdAsync(contaId);
        ArgumentNullException.ThrowIfNull(conta);
        var transacoes = conta.TransacoesComoDestino;
        _logger.LogInformation("BankSystem.Api: Buscando transacoes da conta como destino por id: {id}", contaId);
        return transacoes.Select(t => new TransacaoViewModel(t));
    }

    public async Task<bool> DepositAsync(Guid contaId, decimal valor)
    {
        var conta = await _contaRepository.GetByIdAsync(contaId);
        ArgumentNullException.ThrowIfNull(conta);

        switch (conta.Tipo)
        {
            case TipoDeConta.Corrente 
                or TipoDeConta.Digital 
                or TipoDeConta.Pagamento : 
                if (valor >= 100) valor += valor * PorcentagemTaxaTransferencia;
                break;
        }
        if (conta.StatusDaConta == StatusDaConta.Ativo)
            conta.Depositar(valor);
        else throw new InvalidOperationException("Conta deve estar ativa para realizar deposito.");
        _logger.LogInformation("BankSystem.Api: Deposito realizado na conta {contaId} com sucesso. Valor: {valor}", contaId, valor);
        return await _contaRepository.UpdateAsync(conta);
    }
    
    public async Task<bool> WithdrawlAsync(Guid contaId, decimal valor)
    {
        var conta = await _contaRepository.GetByIdAsync(contaId);
        ArgumentNullException.ThrowIfNull(conta);
        if (conta.StatusDaConta != StatusDaConta.Ativo)
            throw new InvalidOperationException("Conta deve estar ativa para realizar saque.");

        switch (conta.Tipo)
        {
            case TipoDeConta.Salario: throw new InvalidOperationException("Conta de salario nao permite saque, apenas transferencia para contas do mesmo dono.");
        }
        
        conta.Sacar(valor);
        _logger.LogInformation("BankSystem.Api: Saque realizado na conta {contaId} com sucesso. Valor: {valor}", contaId, valor);
        return await _contaRepository.UpdateAsync(conta);
    }

    public async Task Transfer(Guid contaOrigemId, Guid contaDestinoId, decimal valor)
    {
        var contaOrigem = await _contaRepository.GetByIdAsync(contaOrigemId);
        var contaDestino = await _contaRepository.GetByIdAsync(contaDestinoId);
        
        if (valor <= 0) throw new InvalidOperationException("Valor da transacao deve ser maior que zero.");
        
        if (contaOrigem == null || contaDestino == null)
            throw new InvalidOperationException("Conta de origem ou destino inexistente.");
        if (contaOrigem.Id == contaDestino.Id) throw new InvalidOperationException("Conta origem e destino iguais.");
        
        if (contaOrigem.StatusDaConta != StatusDaConta.Ativo || contaDestino.StatusDaConta != StatusDaConta.Ativo)
            throw new InvalidOperationException("Ambas as contas devem estar ativas para realizar a transferencia.");
        
        switch (contaOrigem.Tipo)
        {
            case TipoDeConta.Salario when contaDestino.Cliente_Id != contaOrigem.Cliente_Id:
                throw new InvalidOperationException(
                    "Conta de destino deve ser do mesmo cliente da conta de origem em transacoes de contas do tipo Salario.");
            case TipoDeConta.Poupanca when contaDestino.Cliente_Id != contaOrigem.Cliente_Id:
                throw new InvalidOperationException(
                    "Conta de destino deve ser do mesmo cliente da conta de origem em transacoes de contas do tipo Poupanca.");
        }

        // testar se, ao a operacao de saque da conta origem ser feita porem dar excecao na operacao de deposito na conta destino ira salvar um estado incorreto na aplicacao
        contaOrigem.Sacar(valor);
        contaDestino.Depositar(valor);
        await _contaRepository.UpdateAsync(contaOrigem);
        await _contaRepository.UpdateAsync(contaDestino);
        _logger.LogInformation("BankSystem.Api: Transferencia realizada entre contas {contaOrigemId} e {contaDestinoId} com sucesso. Valor: {valor}", contaOrigemId, contaDestinoId, valor);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("BankSystem.Api: Deletando conta por id: {id}", id);
        return await _contaRepository.DeleteAsync(id);
    }

    public ContaViewModel ContaToDto(Conta conta)
    {
        ArgumentNullException.ThrowIfNull(conta);
        return new ContaViewModel(conta);
    }
}