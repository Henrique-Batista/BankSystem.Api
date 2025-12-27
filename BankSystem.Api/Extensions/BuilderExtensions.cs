using System.Text.Json.Serialization;
using BankSystem.Api.ExceptionHandler;
using BankSystem.Application.Repositories;
using BankSystem.Application.Services;
using BankSystem.Domain.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;

namespace BankSystem.Api.Extensions;

public static class BuilderExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IRepository<Conta>, ContaRepository>();
        services.AddScoped<IRepository<Transacao>, TransacaoRepository>();
        services.AddScoped<IRepository<Cliente>, ClienteRepository>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<ITransacaoService, TransacaoService>();
        services.AddScoped<IContaService, ContaService>();
    }

    public static void AddExceptionHandling(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

    }

    public static void AddLoggingStrategy(this IServiceCollection services)
    {
        services.AddSerilog(loggingBuilder => loggingBuilder
            .WriteTo.File("logs/log.txt", LogEventLevel.Information), 
            writeToProviders: true);
    }
    public static void AddAplicationControllers(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
    }
}