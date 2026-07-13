using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Orders.PublicApi;

public interface IOrderService
{
    Task<OrderShippingInfo?> GetOrderForShippingAsync(Guid orderId);

    /// <summary>
    /// 预订订单库存
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    Task<ReserveOrderStockResult> ReserveOrderStockAsync(Guid orderId);
}