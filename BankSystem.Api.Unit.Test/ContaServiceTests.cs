using System.Reflection;
using BankSystem.Application.DTOs;
using BankSystem.Application.Services;
using BankSystem.Application.Repositories;
using BankSystem.Domain.Models;
using BankSystem.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BankSystem.Api.Unit.Test;

public class ContaServiceTests
{
    private readonly Mock<IContaRepository> _contaRepositoryMock;
    private readonly Mock<IClienteService> _clienteServiceMock;
    private readonly Mock<IRepository<Cliente>> _clienteRepositoryMock;
    private readonly ContaService _contaService;
    private readonly ClienteService _clienteService;
    private readonly Mock<ILogger<ContaService>> _contalogger = new();
    private readonly Mock<ILogger<ClienteService>> _clientelogger = new();
    private readonly Mock<IContaService> _contaServiceMock;

    public ContaServiceTests()
    {
        _clienteRepositoryMock = new Mock<IRepository<Cliente>>();
        _contaRepositoryMock = new Mock<IContaRepository>();
        _clienteServiceMock = new Mock<IClienteService>();
        _contaServiceMock = new Mock<IContaService>();
        _contaService = new(_contaRepositoryMock.Object, _clienteServiceMock.Object, _contalogger.Object);
        _clienteService = new(_clienteRepositoryMock.Object, _clientelogger.Object);
    }

    [Fact]
    public async Task AddContaAsync_ShouldThrowException_WhenClienteDoesNotExist()
    {
        // Arrange
        var cliente = new Cliente("Teste", "123.456.789-00", new DateOnly(1990, 1, 1));
        var contaDto = new ContaInputModel(TipoDeConta.Salario, cliente.Id);
        
        _clienteServiceMock.Setup(x => x.GetByIdAsync(cliente.Id))
            .ReturnsAsync((ClienteViewModel?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _contaService.AddContaAsync(contaDto));
    }

    [Fact]
    public async Task AddContaAsync_ShouldCreateAccount_WhenClienteExists()
    {
        // Arrange
        var cliente = new Cliente("Teste", "123.456.789-00", new DateOnly(2000, 1, 1));
        var contaDto = new ContaInputModel(TipoDeConta.Poupanca, cliente.Id);
        
        var clienteViewModel = new ClienteViewModel(cliente.Id, "Teste", "123.456.789-00", "01/01/2000");

        _clienteServiceMock.Setup(x => x.GetByIdAsync(cliente.Id))
            .ReturnsAsync(clienteViewModel);
            
        _contaRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Conta>()))
            .ReturnsAsync(Guid.NewGuid());

        // Act
        var result = await _contaService.AddContaAsync(contaDto);

        // Assert
        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task ActivateAccountAsync_ShouldReturnTrue_WhenDataMatches()
    {
        // Arrange
        var dataNascimento = new DateOnly(2000, 1, 1);
        var cpf = "123.456.789-00";
        var nome = "Nome Teste";

        // Criando o Cliente de Domínio e a Conta
        var cliente = new Cliente(nome, cpf, dataNascimento);

        var conta = new Conta(TipoDeConta.Corrente, cliente.Id); 

        _contaRepositoryMock.Setup(x => x.GetByIdAsync(conta.Id))
            .ReturnsAsync(conta);
        _contaRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Conta>()))
            .ReturnsAsync(true);
        
        _clienteServiceMock.Setup(x => x.GetByIdAsync(cliente.Id)).ReturnsAsync(_clienteService.ClientToDto(cliente)); 

        // InputModel para validação (DataNascimento como string)
        var clienteInput = new ClienteInputModel(nome, cpf, dataNascimento.ToString());

        // Act
        var result = await _contaService.ActivateAccountAsync(conta.Id, clienteInput);

        // Assert
        Assert.True(result);
        Assert.Equal(StatusDaConta.Ativo, conta.StatusDaConta);
    }

    [Fact]
    public async Task DepositAsync_ShouldThrow_WhenAccountIsInactive()
    {
        // Arrange
        var conta = new Conta(TipoDeConta.Corrente, Guid.NewGuid());
        
        _contaRepositoryMock.Setup(x => x.GetByIdAsync(conta.Id))
            .ReturnsAsync(conta);

        // Act & Assert
        // A lógica atual do seu serviço lança exceção se não for ativo
        await Assert.ThrowsAsync<InvalidOperationException>(() => _contaService
            .DepositAsync(conta.Id, 100));
    }
}
