namespace BankSystem.Domain.Models;

public sealed class Cliente : Entity
{
    private string _cpf = String.Empty;
    private Cliente()
    {
        
    }
    
    public Cliente(string nome, string cpf, DateOnly dataNascimento)
    {
        Nome = nome;
        Cpf = cpf;
        DataNascimento = dataNascimento;
    }

    public string Nome { get; set; } = String.Empty;
    
    public string Cpf
    {
        get => _cpf;
        private set
        {
            var valores = value.Split(".");
            
            if (string.IsNullOrEmpty(value) || value.Length != 14 || valores.Length != 3) throw new InvalidOperationException("CPF invalido!");
        
            _cpf = value;
        }
    }

    public DateOnly DataNascimento { get; private set; }
    public ICollection<Conta> Contas { get; private set; } = new List<Conta>();
}