using eShop.Shipping.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace eShop.Shipping.Api;

public static class Endpoints
{
    internal sealed record CreateOrderRequest(string CustomerName, string ShippingAddress, string ProductName, int Quantity, decimal TotalPrice);
    public static void MapShippingEndpoints(this WebApplication app)
    {
        app.MapGet("shipments/{id:guid}", async (Guid id, ShippingDbContext dbContext) =>
        {
            var shipmentRecord = await dbContext.ShipmentRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (shipmentRecord == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(shipmentRecord);
        });
    }
}
