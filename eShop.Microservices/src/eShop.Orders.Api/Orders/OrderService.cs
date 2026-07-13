using eShop.Orders.Api.Infrastructure;
using eShop.Orders.PublicApi;
using eShop.Products.PublicApi;
using Microsoft.EntityFrameworkCore;

namespace eShop.Orders.Api.Orders;

internal class OrderService(OrdersDbContext dbContext, IProductService productService) : IOrderService
{
    public async Task<OrderShippingInfo?> GetOrderForShippingAsync(Guid orderId)
    {
        var order = await dbContext.Orders
            .AsNoTracking()
            .Where(o => o.Id == orderId)
            .Select(o => new OrderShippingInfo(o.Id, o.ShippingAddress))
            .FirstOrDefaultAsync();

        return order;
    }

    public async Task<ReserveOrderStockResult> ReserveOrderStockAsync(Guid orderId)
    {
        var order = await dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            return new ReserveOrderStockResult(ReserveOrderStockStatus.Failed);
        }

        var itemsToReserve = order.Items
            .Select(item => item.ProductId)
            .ToArray();

        // Call the (intentionally not implemented) product service
        await productService.ReserveProductsAsync(itemsToReserve);

        return new ReserveOrderStockResult(ReserveOrderStockStatus.Successful);
    }
}
