using eShop.Orders.PublicApi;
using eShop.Shipping.Api.IntegrationEvents;
using eShop.Shipping.Api.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace eShop.Shipping.Api;

internal class ShipmentRecordScheduledConsumer(
    IOrderService orderService,
    ShippingDbContext shippingDbContext,
    ILogger<ShipmentRecordScheduledConsumer> logger)
    : IConsumer<ShipmentRecordScheduledEvent>
{
    public async Task Consume(ConsumeContext<ShipmentRecordScheduledEvent> context)
    {
        logger.LogInformation(
            "收到 ShipmentRecordId: {ShipmentRecordId} 的 ShipmentRecordScheduledEvent",
            context.Message.ShipmentRecordId);

        var shipmentRecord = await shippingDbContext.ShipmentRecords
            .FirstOrDefaultAsync(sr => sr.Id == context.Message.ShipmentRecordId);

        if (shipmentRecord == null)
        {
            logger.LogError("未找到 ID 为 {ShipmentRecordId} 的 ShipmentRecord", context.Message.ShipmentRecordId);
            // Optionally, could move the message to an error queue or take other corrective action.
            return;
        }

        try
        {
            var result = await orderService.ReserveOrderStockAsync(shipmentRecord.OrderId);

            // 这可能涉及更新 ShipmentRecord 状态、发布更多事件等。
            logger.LogInformation(
                "订单Id: {OrderId} 的库存预订结果: {Status}",
                shipmentRecord.OrderId,
                result.Status);

            if (result.Status == ReserveOrderStockStatus.Failed)
            {
                logger.LogWarning("订单Id: {OrderId} 的库存预订失败", shipmentRecord.OrderId);
                // Handle failure case - e.g., update shipment status to Failed, notify someone.
            }
            else
            {
                // Handle success case - e.g., update shipment status to AwaitingPickup?
                shipmentRecord.Status = ShipmentStatus.Shipped;

                await shippingDbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error reserving stock for OrderId {OrderId}", shipmentRecord.OrderId);
            // Handle exceptions, potentially re-queue the message depending on the error.
            throw; // Re-throw to allow MassTransit to handle retries/error queues based on configuration.
        }
    }
}
