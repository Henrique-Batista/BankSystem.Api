using BankSystem.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Api.ExceptionHandler;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception, 
            "BankSystem.Api: Ocorreu um erro inesperado: {Message}", 
            exception.Message);
        
        var problemDetails = exception switch 
        {
            ArgumentNullException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Requisição Inválida",
                Detail = exception.Message,
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1"
            },
            InvalidCpfException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "CPF Inválido",
                Detail = exception.Message
            },
            InsufficientBalanceException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Saldo Insuficiente",
                Detail = "Nao ha saldo suficiente na conta de origem da transacao para realizar essa operacao.",
            },
            InvalidOperationException => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Operacao Invalida",
                Detail = exception.Message,
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8"
            },
            DbUpdateException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Erro ao Atualizar o Banco de Dados",
                Detail = "Ocorreu um erro ao tentar atualizar o banco de dados. Verifiqueos dados enviados para localizar registros duplicados.",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1"
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Erro Interno do Servidor",
                Detail = "Ocorreu um erro no processamento da sua requisição.",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
            }
        };

        httpContext.Response.StatusCode = problemDetails.Status!.Value;

        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; 
    }
}