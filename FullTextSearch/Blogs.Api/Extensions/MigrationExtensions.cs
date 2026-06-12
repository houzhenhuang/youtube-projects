using Blogs.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace Blogs.Api.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<BlogsDbContext>();

        dbContext.Database.Migrate();
    }
}