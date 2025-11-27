using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankSystem.Domain.ValueObjects;

namespace BankSystem.Domain.Models;

public sealed class Conta : Entity
{
    private Conta()
    {
        
    }
    
    public Conta(TipoDeConta tipo, Guid clienteId)
    {
        Tipo = tipo;
        StatusDaConta = StatusDaConta.Inativo;
        Cliente_Id = clienteId;
    }

    public int Numero { get; private set; }
    public decimal Saldo { get; private set; }
    public Guid Cliente_Id { get; private set;}
    
    [EnumDataType(typeof(StatusDaConta))]
    public StatusDaConta StatusDaConta { get; set; }
    
    [EnumDataType(typeof(TipoDeConta))]
    public TipoDeConta Tipo { get; private set;}
    public Cliente? Cliente { get; private set; }
    
    public ICollection<Transacao> TransacoesComoOrigem { get; private set; } = new List<Transacao>();
    public ICollection<Transacao> TransacoesComoDestino { get; private set; } = new List<Transacao>();
    
    [NotMapped]
    public IEnumerable<Transacao> Transacoes => TransacoesComoOrigem.Concat(TransacoesComoDestino);
}