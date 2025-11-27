using BankSystem.Domain.ValueObjects;

namespace BankSystem.Application.DTOs;

public sealed record ContaInputModel(TipoDeConta Tipo, Guid ClienteId);