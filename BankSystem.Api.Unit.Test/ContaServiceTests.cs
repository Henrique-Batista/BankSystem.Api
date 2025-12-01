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
    private readonly Mock<IRepository<Conta>> _contaRepositoryMock;
    private readonly Mock<IClienteService> _clienteServiceMock;
    private readonly Mock<ILogger<ContaService>> _logger = new();
    private readonly ContaService _contaService;

    public ContaServiceTests()
    {
        _contaRepositoryMock = new Mock<IRepository<Conta>>();
        _clienteServiceMock = new Mock<IClienteService>();
        _contaService = new ContaService(_contaRepositoryMock.Object, _clienteServiceMock.Object, _logger.Object);
    }

    [Fact]
    public async Task AddContaAsync_ShouldThrowException_WhenClienteDoesNotExist()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var contaDto = new ContaInputModel(TipoDeConta.Salario, clienteId);
        
        _clienteServiceMock.Setup(x => x.GetByIdAsync(clienteId))
            .ReturnsAsync((ClienteViewModel?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _contaService.AddContaAsync(contaDto));
        _contaRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Conta>()), Times.Never);
    }

    [Fact]
    public async Task AddContaAsync_ShouldCreateAccount_WhenClienteExists()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var contaDto = new ContaInputModel(TipoDeConta.Poupanca, clienteId);
        
        // Corrigido: construtor do ViewModel
        var clienteViewModel = new ClienteViewModel(clienteId, "Teste", "123.456.789-00", "01/01/2000");

        _clienteServiceMock.Setup(x => x.GetByIdAsync(clienteId))
            .ReturnsAsync(clienteViewModel);
            
        _contaRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Conta>()))
            .ReturnsAsync(Guid.NewGuid());

        // Act
        var result = await _contaService.AddContaAsync(contaDto);

        // Assert
        Assert.NotNull(result);
        _contaRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Conta>()), Times.Once);
    }
    
    [Fact]
    public async Task ActivateAccountAsync_ShouldReturnTrue_WhenDataMatches()
    {
        // Arrange
        var contaId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var dataNascimento = new DateOnly(2000, 1, 1);
        var cpf = "123.456.789-00";
        var nome = "Nome Teste";

        // Criando o Cliente de Domínio e a Conta
        var cliente = new Cliente(nome, cpf, dataNascimento);

        var conta = new Conta(TipoDeConta.Corrente, clienteId); 
        
        // Importante: Setar a propriedade de navegação Cliente dentro da Conta
        SetPrivateProperty(conta, "Cliente", cliente);

        _contaRepositoryMock.Setup(x => x.GetByIdAsync(contaId)).ReturnsAsync(conta);
        _contaRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Conta>())).ReturnsAsync(true);

        // InputModel para validação (DataNascimento como string)
        var clienteInput = new ClienteInputModel(nome, cpf, dataNascimento.ToString());

        // Act
        var result = await _contaService.ActivateAccountAsync(contaId, clienteInput);

        // Assert
        Assert.True(result);
        Assert.Equal(StatusDaConta.Ativo, conta.StatusDaConta);
        _contaRepositoryMock.Verify(x => x.UpdateAsync(conta), Times.Once);
    }

    [Fact]
    public async Task DepositAsync_ShouldThrow_WhenAccountIsInactive()
    {
        // Arrange
        var contaId = Guid.NewGuid();
        var conta = new Conta(TipoDeConta.Corrente, Guid.NewGuid()) 
        { 
            StatusDaConta = StatusDaConta.Inativo 
        };
        
        _contaRepositoryMock.Setup(x => x.GetByIdAsync(contaId)).ReturnsAsync(conta);

        // Act & Assert
        // A lógica atual do seu serviço lança exceção se não for ativo
        await Assert.ThrowsAsync<InvalidOperationException>(() => _contaService.DepositAsync(contaId, 100));
    }

    private void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        prop?.SetValue(obj, value);
    }
}
