using eShop.Products.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Products.Api;

public static class ProductsEndpoints
{
    // Mock data store
    private static readonly List<Product> Products =
    [
        new() { Id = 1, Name = "Laptop Pro", Price = 1200.00m, AvailableQuantity = 10 },
        new() { Id = 2, Name = "Wireless Mouse", Price = 25.50m, AvailableQuantity = 50 },
        new() { Id = 3, Name = "Mechanical Keyboard", Price = 89.99m, AvailableQuantity = 25 },
        new() { Id = 4, Name = "4K Monitor", Price = 350.00m, AvailableQuantity = 5 },
        new() { Id = 5, Name = "Webcam HD", Price = 45.75m, AvailableQuantity = 30 }
    ];

    public static IEndpointRouteBuilder MapProductsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var productsGroup = endpoints.MapGroup("api/products").WithTags("Products");

        // GET /products/{id}
        productsGroup.MapGet("/{id:int}", async (int id) =>
        {
            var product = Products.FirstOrDefault(p => p.Id == id);
            await Task.Delay(1000);
            return product is not null ? Results.Ok(product) : Results.NotFound();
        })
        .WithName("GetProductById")
        .Produces<Product>()
        .Produces(StatusCodes.Status404NotFound);

        // GET /products/batch
        productsGroup.MapGet("/batch", async ([FromQuery] int[] ids) =>
        {
            var foundProducts = Products.Where(p => ids.Contains(p.Id)).ToList();
            await Task.Delay(250);
            return Results.Ok(foundProducts);
        })
        .WithName("GetProductsByIds")
        .Produces<List<Product>>();

        // POST /products/reserve
        productsGroup.MapPost("/reserve", (ReserveProductsRequest request) =>
        {
            // Simulate some processing time
            // 在实际场景中，这将涉及检查库存、更新数量等。
            Console.WriteLine($"收到预订产品的请求: {string.Join(", ", request.ProductIds)}");

            // 模拟预订逻辑失败/不完整
            // throw new NotImplementedException("Product reservation logic is not implemented yet.");

            // If implemented, would likely return Results.Ok() or Results.NoContent()
        })
        .WithName("ReserveProducts")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status501NotImplemented);


        return endpoints;
    }
}

internal record ReserveProductsRequest(int[] ProductIds);