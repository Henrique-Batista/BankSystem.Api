using BankSystem.Application.DTOs;
using BankSystem.Application.Services;
using BankSystem.Application.Repositories;
using BankSystem.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace BankSystem.Api.Unit.Test;

public sealed class ClienteServiceTests
{
    private readonly Mock<IRepository<Cliente>> _clienteRepositoryMock;
    private readonly ClienteService _clienteService;
    private readonly Mock<ILogger<ClienteService>> _logger = new();

    public ClienteServiceTests()
    {
        _clienteRepositoryMock = new Mock<IRepository<Cliente>>();
        _clienteService = new ClienteService(_clienteRepositoryMock.Object, _logger.Object);
    }

    [Fact]
    public async Task GetAllClientsAsync_ShouldReturnListOfClients_WhenClientsExist()
    {
        // Arrange
        var clientesList = new List<Cliente>
        {
            new Cliente("João Silva", "123.456.789-00", new DateOnly(1990, 1, 1)),
            new Cliente("Maria Souza", "987.654.321-11", new DateOnly(1995, 5, 10))
        };

        _clienteRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(clientesList);

        // Act
        var result = await _clienteService.GetAllClientsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _clienteRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }
    
    [Fact]
    public async Task GetByIdAsync_ShouldReturnClienteViewModel_WhenClienteExists()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var cliente = new Cliente("João Silva", "123.456.789-00", new DateOnly(1990, 1, 1));
        

        _clienteRepositoryMock.Setup(x => x.GetByIdAsync(clienteId))
            .ReturnsAsync(cliente);

        // Act
        var result = await _clienteService.GetByIdAsync(clienteId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("João Silva", result.Nome);
        _clienteRepositoryMock.Verify(x => x.GetByIdAsync(clienteId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowException_WhenClienteDoesNotExist()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        _clienteRepositoryMock.Setup(x => x.GetByIdAsync(clienteId))
            .ReturnsAsync((Cliente?)null);

        // Act & Assert
        // O seu ClienteService usa ArgumentNullException.ThrowIfNull, então esperamos uma exceção
        await Assert.ThrowsAsync<ArgumentNullException>(() => _clienteService.GetByIdAsync(clienteId));
    }

    [Fact]
    public async Task AddClientAsync_ShouldReturnClientId_WhenInputIsValid()
    {
        // Arrange
        // ClienteInputModel espera string para DataNascimento
        var input = new ClienteInputModel("Carlos Lima", "111.222.333-44", "2000-01-01");
        
        // Simulando o retorno do ID gerado
        _clienteRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Cliente>()))
            .ReturnsAsync(Guid.NewGuid());

        // Act
        var result = await _clienteService.AddClientAsync(input);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Guid>(result);
        _clienteRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Cliente>()), Times.Once);
    }

    [Fact]
    public async Task ChangeClientNameAsync_ShouldReturnTrue_WhenClientExistsAndNameIsDifferent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cliente = new Cliente("Nome Antigo", "111.111.111-11", new DateOnly(1990, 1, 1));
        
        _clienteRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(cliente);
        _clienteRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Cliente>())).ReturnsAsync(true);

        // Act
        var result = await _clienteService.ChangeClientNameAsync(id, "Nome Novo");

        // Assert
        Assert.True(result);
        Assert.Equal("Nome Novo", cliente.Nome);
        _clienteRepositoryMock.Verify(x => x.UpdateAsync(cliente), Times.Once);
    }
    
    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenRepositoryReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        _clienteRepositoryMock.Setup(x => x.DeleteAsync(id)).ReturnsAsync(true);

        // Act
        var result = await _clienteService.DeleteAsync(id);

        // Assert
        Assert.True(result);
        _clienteRepositoryMock.Verify(x => x.DeleteAsync(id), Times.Once);
    }
}
