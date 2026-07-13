using eShop.Shipping.Api.IntegrationEvents;
using eShop.Shipping.Api.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace eShop.Shipping.Api.BackgroundServices;

/// <summary>
/// 待发货扫描服务
/// </summary>
/// <param name="serviceProvider"></param>
/// <param name="logger"></param>
public class PendingShipmentScannerService(
    IServiceProvider serviceProvider,
    ILogger<PendingShipmentScannerService> logger) : BackgroundService
{
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(10)); // 每 10 秒扫描一次
    private readonly ConcurrentDictionary<Guid, int> _pendingShipmentCounts = new();
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            await ScanAndPublishPendingShipments(stoppingToken);
        }
    }

    /// <summary>
    /// 扫描并发布待发货的货件
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task ScanAndPublishPendingShipments(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ShippingDbContext>();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        // 获取可跟踪实体以允许状态更新
        var pendingShipments = await dbContext.ShipmentRecords
            .Where(s => s.Status == ShipmentStatus.Pending)
            .ToListAsync(cancellationToken);

        if (!pendingShipments.Any())
        {
            return;
        }

        logger.LogDebug("发现 {Count} 个待处理的货件。", pendingShipments.Count);

        foreach (var shipment in pendingShipments)
        {
            var seenCount = _pendingShipmentCounts.AddOrUpdate(shipment.Id, 1, (key, count) => count + 1);

            if (seenCount >= 2)
            {
                logger.LogWarning(
                    "货件 {ShipmentId} 已检测为待处理状态 {SeenCount} 次。标记为失败。",
                    shipment.Id, seenCount);

                // 之前见过此待处理的货件，标记为失败
                shipment.Status = ShipmentStatus.Failed;
                _pendingShipmentCounts.TryRemove(shipment.Id, out _);
            }
            else
            {
                logger.LogInformation("发布 ShipmentId: {ShipmentId} 的 ShipmentRecordScheduledEvent", shipment.Id);

                // 第一次看到待发货, publish the event
                var shipmentScheduled = new ShipmentRecordScheduledEvent(shipment.Id);
                await publishEndpoint.Publish(shipmentScheduled, cancellationToken);
            }
        }

        // Save changes if any status was updated to Failed
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
