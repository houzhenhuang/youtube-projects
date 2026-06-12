namespace Shipping.Api.Shipments;

internal enum ShipmentStatus
{
    /// <summary>
    /// 待发货（已创建运单，等待实际发货）
    /// </summary>
    Pending,
    /// <summary>
    /// 已发货（已交接给物流，正在运输中）
    /// </summary>
    Shipped,
    /// <summary>
    /// 已送达（已完成配送）
    /// </summary>
    Delivered
}