namespace eShop.Shipping.Api.Models;

public enum ShipmentStatus
{
    Pending,
    /// <summary>
    /// 已发货
    /// </summary>
    Shipped,
    Delivered,
    Failed
}