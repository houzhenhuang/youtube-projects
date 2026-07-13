using eShop.Orders.PublicApi;
using eShop.Shipping.Api.Models;
using MassTransit;

namespace eShop.Shipping.Api;

internal class OrderCreatedConsumer(IOrderService orderService ,ShippingDbContext dbContext) : IConsumer<OrderCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
    {
        var orderShippingInfo = await orderService.GetOrderForShippingAsync(context.Message.OrderId);

        if (orderShippingInfo is null)
        {
            throw new InvalidOperationException($"订单 {context.Message.OrderId} 不存在");
        }

        var shipmentRecord = new ShipmentRecord
        {
            OrderId = orderShippingInfo.OrderId,
            ShippingAddress = orderShippingInfo.ShippingAddress,
            TrackingNumber = $"TRACK-{Guid.NewGuid():N}",
            CreatedAt = DateTime.UtcNow,
            Status = ShipmentStatus.Pending
        };

        dbContext.ShipmentRecords.Add(shipmentRecord);
        await dbContext.SaveChangesAsync();
    }
}
