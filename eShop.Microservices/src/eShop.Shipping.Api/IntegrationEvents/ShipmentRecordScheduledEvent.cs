namespace eShop.Shipping.Api.IntegrationEvents;

/// <summary>
/// 出货记录预定
/// </summary>
/// <param name="ShipmentRecordId"></param>
public record ShipmentRecordScheduledEvent(Guid ShipmentRecordId);
