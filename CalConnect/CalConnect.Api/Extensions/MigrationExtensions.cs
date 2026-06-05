using CalConnect.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace CalConnect.Api.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this WebApplication app)
    {
        //using var scope = app.Services.CreateScope();
        //var serviceProvider = scope.ServiceProvider;

        //var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        //if (dbContext.Database.EnsureCreated())
        //{
        //    return;
        //}

        //dbContext.Database.Migrate();
    }
}