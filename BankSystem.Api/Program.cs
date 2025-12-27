using System.Text.Json.Serialization;
using BankSystem.Api.ExceptionHandler;
using BankSystem.Application.Repositories;
using BankSystem.Application.Services;
using BankSystem.Domain.Models;
using BankSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddOpenApi();

builder.AddNpgsqlDbContext<BankDbContext>("postgresdb");
builder.Services.AddSerilog(loggingBuilder => loggingBuilder
        .WriteTo.File("logs/log.txt", LogEventLevel.Information).WriteTo.Console());


builder.Services.AddScoped<IRepository<Conta>, ContaRepository>();
builder.Services.AddScoped<IRepository<Transacao>, TransacaoRepository>();
builder.Services.AddScoped<IRepository<Cliente>, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ITransacaoService, TransacaoService>();
builder.Services.AddScoped<IContaService, ContaService>();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();



var app = builder.Build();

app.UseExceptionHandler();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<BankDbContext>();
    context.Database.Migrate();
    DatabaseSeeder.Seed(context);
}
app.MapOpenApi();
app.MapScalarApiReference();

app.MapGet("api/healthcheck", (BankDbContext context) =>
{
    var databaseCanConnect = context.Database.CanConnectAsync().GetAwaiter().GetResult();
    if (!databaseCanConnect) return Results.InternalServerError("Cannot connect to database after 15 seconds!");
    return Results.Ok("Database server working!");
}).WithName("HealthCheck");

app.Run();