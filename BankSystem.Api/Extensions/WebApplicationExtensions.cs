using BankSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Api.Extensions;

public static class WebApplicationExtensions
{
    public static void UseAppHealthCheck(this WebApplication app)
    {
        app.MapGet("api/healthcheck", (BankDbContext context) =>
        {
            var databaseCanConnect = context.Database.CanConnectAsync().GetAwaiter().GetResult();
            if (!databaseCanConnect) return Results.InternalServerError("Cannot connect to database after 15 seconds!");
            return Results.Ok("Database server working!");
        }).WithName("HealthCheck");
    }

    public static void InitDatabaseAtDevelopment(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BankDbContext>();
            context.Database.Migrate();
            DatabaseSeeder.Seed(context);
        }
    }
}