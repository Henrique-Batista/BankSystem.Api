using Bogus;
using BankSystem.Domain.Models;
using BankSystem.Domain.ValueObjects;

namespace BankSystem.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static void Seed(BankDbContext context)
    {
        if (context.Clientes.Any())
            return;

        var clienteFaker = new Faker<Cliente>("pt_BR")
            .CustomInstantiator(f => new Cliente(
                nome: f.Name.FullName(),
                cpf: GerarCpfValido(),
                dataNascimento: DateOnly.FromDateTime(f.Date.Past(30, DateTime.Now.AddYears(-18)))
            ));

        var clientes = clienteFaker.Generate(10);
        context.Clientes.AddRange(clientes);
        context.SaveChanges();

        var contaFaker = new Faker<Conta>("pt_BR")
            .CustomInstantiator(f => new Conta(
                tipo: f.PickRandom<TipoDeConta>(),
                clienteId: f.PickRandom(clientes).Id
            ))
            .RuleFor(c => c.StatusDaConta, f => f.PickRandom<StatusDaConta>());

        var contas = contaFaker.Generate(25);
        context.Contas.AddRange(contas);
        context.SaveChanges();

        var contasAtivas = contas.Where(c => c.StatusDaConta == StatusDaConta.Ativo).ToList();
        
        if (contasAtivas.Count < 2)
            return;

        foreach (var conta in contasAtivas)
        {
            typeof(Conta).GetProperty("Saldo")!
                .SetValue(conta, new Faker().Random.Decimal(1000, 50000));
        }
        context.SaveChanges();

        var transacaoFaker = new Faker<Transacao>("pt_BR")
            .CustomInstantiator(f =>
            {
                var origem = f.PickRandom(contasAtivas);
                var destino = f.PickRandom(contasAtivas.Where(c => c.Id != origem.Id).ToList());
                
                return new Transacao(
                    tipo: f.PickRandom<TipoDeTransacao>(),
                    valor: f.Random.Decimal(10, 500),
                    contaOrigemId: origem.Id,
                    contaDestinoId: destino.Id
                );
            });

        var transacoes = transacaoFaker.Generate(50);
        context.Transacoes.AddRange(transacoes);
        context.SaveChanges();
    }

    private static string GerarCpfValido()
    {
        var random = new Random();
        var cpf = new int[11];

        for (int i = 0; i < 9; i++)
            cpf[i] = random.Next(0, 9);

        int soma = 0;
        for (int i = 0; i < 9; i++)
            soma += cpf[i] * (10 - i);
        
        int resto = soma % 11;
        cpf[9] = resto < 2 ? 0 : 11 - resto;

        soma = 0;
        for (int i = 0; i < 10; i++)
            soma += cpf[i] * (11 - i);
        
        resto = soma % 11;
        cpf[10] = resto < 2 ? 0 : 11 - resto;

        return $"{cpf[0]}{cpf[1]}{cpf[2]}.{cpf[3]}{cpf[4]}{cpf[5]}.{cpf[6]}{cpf[7]}{cpf[8]}-{cpf[9]}{cpf[10]}";
    }
}
