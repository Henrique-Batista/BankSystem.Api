using BankSystem.Domain.ValueObjects;

namespace BankSystem.Application.DTOs;

public sealed record TransacaoInputModel(TipoDeTransacao Tipo, decimal Valor, Guid ContaOrigemId, Guid ContaDestinoId);