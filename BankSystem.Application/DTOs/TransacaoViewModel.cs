using BankSystem.Domain.Models;
using BankSystem.Domain.ValueObjects;

namespace BankSystem.Application.DTOs;

public sealed record TransacaoViewModel(
    Guid Id,
    TipoDeTransacao Tipo,
    decimal Valor,
    DateTime DataHora,
    Guid ContaOrigemId,
    Guid ContaDestinoId)
{
    public TransacaoViewModel(Transacao transacao) : this(transacao.Id, transacao.Tipo, transacao.Valor,
        transacao.Data_Hora, transacao.Conta_Origem_Id, transacao.Conta_Destino_Id)
    {
    }
}