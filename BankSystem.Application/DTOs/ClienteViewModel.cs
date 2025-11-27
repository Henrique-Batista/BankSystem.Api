using BankSystem.Domain.Models;

namespace BankSystem.Application.DTOs;

public sealed record ClienteViewModel(Guid Id, string Nome, string Cpf, string DataNascimento)
{
    public ClienteViewModel(Cliente cliente) : this(
        cliente.Id,
        cliente.Nome,
        cliente.Cpf,
        cliente.DataNascimento.ToString("dd/MM/yyyy"))
    {
    }
}