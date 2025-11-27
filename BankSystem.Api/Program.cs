using BankSystem.Api;
using BankSystem.Application.Repositories;
using BankSystem.Application.Services;
using BankSystem.Domain.Models;
using BankSystem.Infrastructure.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddDbContext<BankDbContext>();
builder.Services.AddSqlServer<BankDbContext>(
    builder.Configuration.GetConnectionString("Default"), 
    options => options.MigrationsAssembly("BankSystem.Api"));

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
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

}


app.MapGet("api/healthcheck", (BankDbContext context) =>
{
    var databaseCanConnect = context.Database.CanConnectAsync().GetAwaiter().GetResult();
    if (!databaseCanConnect) return Results.InternalServerError("Cannot connect to database after 15 seconds!");
    return Results.Ok("Database server working!");
}).WithName("HealthCheck");

app.Run();

//TODO: Create business logic for operations
//TODO: Create unit tests
//TODO: Create authorization for operations