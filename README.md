<img width="800" height="800" alt="SP-Studio" src="https://github.com/user-attachments/assets/1526f844-3e95-4b55-b113-21d102059ec7" />


# BankSystem.Api

Sistema bancário desenvolvido em .NET 9 com Entity Framework Core, seguindo princípios de Clean Architecture e Domain-Driven Design (DDD).

## 🚀 Funcionalidades Principais

### Gestão de Clientes
- Criar, consultar, atualizar e deletar clientes
- Validação de CPF
- Listar contas de um cliente específico
- Alterar nome do cliente

### Gestão de Contas
- Criar contas bancárias (Corrente, Poupança, Salário, Pagamento, Digital)
- Ativar/Desativar contas
- Consultar saldo e transações
- Validação de saldo antes de operações
- Status de conta (Ativo/Inativo)

### Transações Financeiras
- Suporte a múltiplos tipos de transação: TED, PIX, Boleto, Cheque
- Transferências entre contas
- Validação de saldo e status das contas
- Histórico completo de transações (origem e destino)

## 🏗️ Arquitetura

O projeto segue Clean Architecture com 4 camadas:

```
BankSystem.Api/              # API REST e Controllers
BankSystem.Application/      # Serviços, DTOs e Repositórios
BankSystem.Domain/          # Entidades, Value Objects e Exceções
BankSystem.Infrastructure/  # DbContext e Data Seeding
```

## 📋 Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/)
- [Entity Framework Core Tools](https://learn.microsoft.com/ef/core/cli/dotnet)
- [Aspire Cli](https://aspire.dev/get-started/install-cli/)

## 🔧 Configuração

### 1. Clone o repositório

```bash
git clone <url-do-repositorio>
cd BankSystem.Api
```

### 2. Rode o Aspire

```bash
aspire run
```

A API estará disponível em `https://localhost:5001` (ou `http://localhost:5000`)

## 📊 Dados de Exemplo (Seed)

Em ambiente de desenvolvimento, o banco de dados é automaticamente populado com dados fictícios gerados pelo [Bogus](https://github.com/bchavez/Bogus):

- **10 clientes** com CPFs válidos
- **25 contas** de tipos variados
- **50 transações** entre contas ativas
- Saldos aleatórios entre R$ 1.000 e R$ 50.000

## 📚 Documentação da API

A documentação interativa da API está disponível através do Scalar:

`https://localhost:5001/scalar/`

### Endpoints Principais

#### Clientes
- `GET /api/clientes` - Lista todos os clientes
- `GET /api/clientes/{id}` - Busca cliente por ID
- `POST /api/clientes` - Cria novo cliente
- `PATCH /api/clientes/{id}` - Altera nome do cliente
- `DELETE /api/clientes/{id}` - Remove cliente
- `GET /api/clientes/{id}/contas` - Lista contas do cliente

#### Contas
- `GET /api/contas` - Lista todas as contas
- `GET /api/contas/{id}` - Busca conta por ID
- `POST /api/contas` - Cria nova conta
- `PATCH /api/contas/{id}/ativar` - Ativa conta
- `PATCH /api/contas/{id}/desativar` - Desativa conta
- `DELETE /api/contas/{id}` - Remove conta
- `GET /api/contas/{id}/transacoes` - Lista todas as transações
- `GET /api/contas/{id}/transacoes/origem` - Transações como origem
- `GET /api/contas/{id}/transacoes/destino` - Transações como destino

#### Transações
- `GET /api/transacoes` - Lista todas as transações
- `GET /api/transacoes/{id}` - Busca transação por ID
- `POST /api/transacoes` - Cria nova transação

#### Health Check
- `GET /api/healthcheck` - Verifica status do banco de dados

## 🧪 Testes

Execute os testes unitários:

```bash
dotnet test
```

## 🛠️ Tecnologias Utilizadas

- **.NET 9**
- **Entity Framework Core 9** - ORM para acesso ao banco de dados
- **PostgreSQL** - Banco de dados
- **Docker** - Criacao e deploy do ambiente de desenvolvimento e producao
- **Bogus** - Geração de dados fictícios
- **Scalar** - Documentação interativa da API
- **xUnit** - Framework de testes
- **Aspire** - Orquestracao e configuracao de servicos

## 📝 Exemplos de Uso

### Criar um Cliente

```bash
curl -X POST https://localhost:5001/api/clientes \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "João Silva",
    "cpf": "123.456.789-00",
    "dataNascimento": "1990-01-01"
  }'
```

### Criar uma Conta

```bash
curl -X POST https://localhost:5001/api/contas \
  -H "Content-Type: application/json" \
  -d '{
    "tipo": "Corrente",
    "clienteId": "guid-do-cliente"
  }'
```

### Realizar uma Transferência

```bash
curl -X POST https://localhost:5001/api/transacoes \
  -H "Content-Type: application/json" \
  -d '{
    "tipo": "PIX",
    "valor": 100.00,
    "contaOrigemId": "guid-conta-origem",
    "contaDestinoId": "guid-conta-destino"
  }'
```

## 🔐 Validações e Regras de Negócio

- CPF deve estar no formato `XXX.XXX.XXX-XX` e ser válido
- Apenas contas ativas podem realizar transações
- Saldo deve ser suficiente para transferências
- Valor de transação deve ser maior que zero
- Cliente não pode ter CPF duplicado

## 📄 Licença

Este projeto está sob licença MIT.
