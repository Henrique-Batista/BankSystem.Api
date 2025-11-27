using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Api;

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
        // 1. Logar o erro original (com Structured Logging)
        _logger.LogError(
            exception, 
            "Ocorreu um erro inesperado: {Message}", 
            exception.Message);

        // 2. Definir o formato de resposta (RFC 7807 Problem Details)
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Erro Interno do Servidor",
            Detail = "Ocorreu um erro no processamento da sua requisição.",
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        // 3. Escrever a resposta como JSON
        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        // Retorna true para indicar que o erro foi tratado
        return true; 
    }
}