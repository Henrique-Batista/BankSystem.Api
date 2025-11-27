using BankSystem.Domain.Models;

namespace BankSystem.Application.DTOs;

public sealed record ContaViewModel (
    Guid Id,
    int Numero,
    decimal Saldo,
    Guid Cliente_Id,
    string StatusDaConta,
    string Tipo)
{
    public ContaViewModel(Conta conta) : this(
        conta.Id,
        conta.Numero,
        conta.Saldo,
        conta.Cliente_Id,
        conta.StatusDaConta.ToString(),
        conta.Tipo.ToString())
    {
        
    }

}