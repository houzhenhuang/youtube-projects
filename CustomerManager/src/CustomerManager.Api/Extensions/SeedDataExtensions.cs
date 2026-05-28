using Microsoft.EntityFrameworkCore;
using OpenApiSamples.Data;

namespace CustomerManager.Api.Extensions;

public static class SeedDataExtensions
{
    public static void ApplySeedData(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Database.EnsureDeleted();
        dbContext.Database.Migrate();
    }
}
