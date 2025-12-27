using System.ComponentModel.DataAnnotations;
using BankSystem.Domain.ValueObjects;

namespace BankSystem.Domain.Models;

/// <summary>
/// Represents a transaction within the banking system.
/// A transaction occurs between two accounts and includes details such as
/// the type of transaction, amount involved, and associated accounts.
/// </summary>
public sealed class Transacao : Entity
{
    private Transacao()
    {
        
    }
    
    public Transacao(TipoDeTransacao tipo, decimal valor, Guid contaOrigemId, Guid contaDestinoId)
    {
        Tipo = tipo;
        if (valor <= 0) throw new ArgumentOutOfRangeException(nameof(valor), "Valor deve ser maior que zero.");
        Valor = valor;
        Data_Hora = DateTime.UtcNow;
        Conta_Origem_Id = contaOrigemId;
        Conta_Destino_Id = contaDestinoId;
    }

    [EnumDataType(typeof(TipoDeTransacao))]
    public TipoDeTransacao Tipo { get; private set; }
    public decimal Valor { get; private set; }
    public DateTime Data_Hora { get; private set; }
    public Guid Conta_Origem_Id { get; private set; }
    public Guid Conta_Destino_Id { get; private set; }
    
    public Conta? ContaOrigem { get; private set; }
    public Conta? ContaDestino { get; private set; }
}