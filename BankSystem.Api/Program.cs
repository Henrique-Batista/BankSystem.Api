using BankSystem.Api.Extensions;
using BankSystem.Infrastructure.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<BankDbContext>("postgresdb");

var services = builder.Services;

services.AddAplicationControllers();
services.AddOpenApi();
services.AddServices();
services.AddLoggingStrategy();
services.AddExceptionHandling();

var app = builder.Build();

app.UseExceptionHandler();
app.MapControllers();

app.InitDatabaseAtDevelopment();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseAppHealthCheck();

app.Run();