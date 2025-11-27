using BankSystem.Application.Repositories;
using BankSystem.Application.Services;
using BankSystem.Domain.Models;
using Moq;

namespace BankSystem.Api.Unit.Test;

public sealed class UnitTest1
{
    private readonly Mock<IContaRepository> _contaRepository;
    private readonly Mock<ITransacaoRepository> _transacaoRepository;
    private readonly Mock<IClienteRepository> _clienteRepository;
    private readonly Mock<IClienteService> _clienteService;
    private readonly Mock<ContaService> _contaService;
    private readonly Mock<TransacaoService> _transacaoService;

    public UnitTest1()
    {
        _clienteRepository = new();
        _transacaoRepository = new();
        _contaRepository = new();
        _contaService = new(_contaRepository.Object, _clienteRepository.Object);
        _clienteService = new(_clienteRepository.Object);
        _transacaoService = new(_transacaoRepository.Object);
    }
    
    [Fact]
    public void Test1()
    {
    }
}