using BankSystem.Application.DTOs;
using BankSystem.Application.Services;
using BankSystem.Application.Repositories;
using BankSystem.Domain.Models;
using BankSystem.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;

namespace BankSystem.Api.Unit.Test;

public class TransacaoServiceTests
{
    private readonly Mock<IRepository<Transacao>> _transacaoRepositoryMock;
    private readonly Mock<IContaService> _contaServiceMock;
    private readonly TransacaoService _transacaoService;
    private Mock<ILogger<TransacaoService>> _logger = new();

    public TransacaoServiceTests()
    {
        _transacaoRepositoryMock = new Mock<IRepository<Transacao>>();
        _contaServiceMock = new Mock<IContaService>();
        _transacaoService = new TransacaoService(_transacaoRepositoryMock.Object, _contaServiceMock.Object, _logger.Object);
    }

    [Fact]
    public async Task AddTransacaoAsync_ShouldThrow_WhenOriginAccountNotFound()
    {
        // Arrange
        var dto = new TransacaoInputModel(TipoDeTransacao.PIX, 100, Guid.NewGuid(), Guid.NewGuid());

        _contaServiceMock.Setup(x => x.GetByIdAsync(dto.ContaOrigemId))
            .ReturnsAsync((ContaViewModel?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _transacaoService.AddTransacaoAsync(dto));
    }

    [Fact]
    public async Task AddTransacaoAsync_ShouldThrow_WhenBalanceInsufficient()
    {
        // Arrange
        var origemId = Guid.NewGuid();
        var destinoId = Guid.NewGuid();
        var dto = new TransacaoInputModel(TipoDeTransacao.PIX, 1000, origemId, destinoId);

        // Simulando contas retornadas pelo serviço
        var contaOrigem = new ContaViewModel(origemId, 1, 50, Guid.NewGuid(), "Ativo", "Corrente"); // Saldo 50 < 1000
        var contaDestino = new ContaViewModel(destinoId, 2, 0, Guid.NewGuid(), "Ativo", "Corrente");

        _contaServiceMock.Setup(x => x.GetByIdAsync(origemId)).ReturnsAsync(contaOrigem);
        _contaServiceMock.Setup(x => x.GetByIdAsync(destinoId)).ReturnsAsync(contaDestino);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _transacaoService.AddTransacaoAsync(dto));
        Assert.Equal("Saldo insuficiente.", ex.Message);
    }

    [Fact]
    public async Task AddTransacaoAsync_ShouldProcess_WhenValid()
    {
        // Arrange
        var origemId = Guid.NewGuid();
        var destinoId = Guid.NewGuid();
        var valor = 100m;
        var dto = new TransacaoInputModel(TipoDeTransacao.PIX, valor, origemId, destinoId);

        var contaOrigem = new ContaViewModel(origemId, 1, 500, Guid.NewGuid(), "Ativo", "Corrente");
        var contaDestino = new ContaViewModel(destinoId, 2, 0, Guid.NewGuid(), "Ativo", "Corrente");

        _contaServiceMock.Setup(x => x.GetByIdAsync(origemId)).ReturnsAsync(contaOrigem);
        _contaServiceMock.Setup(x => x.GetByIdAsync(destinoId)).ReturnsAsync(contaDestino);
        
        _transacaoRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Transacao>()))
            .ReturnsAsync(Guid.NewGuid());

        // Act
        var result = await _transacaoService.AddTransacaoAsync(dto);

        // Assert
        Assert.NotNull(result);
        // Verifica se chamou o serviço de transferência de fundos
        _contaServiceMock.Verify(x => x.Transfer(origemId, destinoId, valor), Times.Once);
        // Verifica se salvou a transação
        _transacaoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Transacao>()), Times.Once);
    }
}
