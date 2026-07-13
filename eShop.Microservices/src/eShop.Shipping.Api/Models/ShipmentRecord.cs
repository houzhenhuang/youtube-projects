namespace eShop.Shipping.Api.Models;

public class ShipmentRecord
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public string ShippingAddress { get; set; } = string.Empty;

    public string TrackingNumber { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public ShipmentStatus Status { get; set; }
}
