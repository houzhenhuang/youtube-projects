namespace Shipping.Api.Shipments;

/// <summary>
/// 订单发货表
/// </summary>
internal sealed class Shipment
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}