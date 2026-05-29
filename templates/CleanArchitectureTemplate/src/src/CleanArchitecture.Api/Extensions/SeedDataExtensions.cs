using CleanArchitecture.Domain.AggregatesEntities.ProductAggregate;
using CleanArchitecture.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Api.Extensions;

public static class SeedDataExtensions
{
    public static async Task InitializeAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.EnsureCreatedAsync();

        await dbContext.Database.MigrateAsync();

        if (await dbContext.Products.AnyAsync())
        {
            return;
        }

        dbContext.Products.AddRange(new List<Product>()
        {
            new (new ProductId(Guid.NewGuid()),"iphone16",new Money("rmb",4999.0m),Sku.Create("APL-IP16-128-BLK")!),
            new (new ProductId(Guid.NewGuid()),"iphone16 pro",new Money("rmb",6999.0m),Sku.Create("APL-IP16-256-BLK")!),
            new (new ProductId(Guid.NewGuid()),"iphone16 pro max",new Money("rmb",8999.0m),Sku.Create("APL-IP16-256-BLK")!),
            new (new ProductId(Guid.NewGuid()),"iphone17",new Money("rmb",5999.0m),Sku.Create("APL-IP17-256-ORG")!),
            new (new ProductId(Guid.NewGuid()),"iphone17 pro",new Money("rmb",7999.0m),Sku.Create("APL-IP17P-256-ORG")!),
            new (new ProductId(Guid.NewGuid()),"iphone17 pro max",new Money("rmb",9999.0m),Sku.Create("APL-IP17PM-256-ORG")!),
        });

        await dbContext.SaveChangesAsync();
    }
}