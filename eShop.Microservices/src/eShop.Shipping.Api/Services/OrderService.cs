using eShop.Orders.PublicApi;

namespace eShop.Shipping.Api.Services;

internal sealed class OrderService(IHttpClientFactory httpClientFactory) : IOrderService
{
    /// <summary>
    /// 获取订单的运输信息
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    public async Task<OrderShippingInfo?> GetOrderForShippingAsync(Guid orderId)
    {
        using var client = httpClientFactory.CreateClient("orders");

        var order = await client.GetFromJsonAsync<OrderShippingInfo>($"api/orders/{orderId}/shipping-info");

        return order;
    }

    public async Task<ReserveOrderStockResult> ReserveOrderStockAsync(Guid orderId)
    {
        using var client = httpClientFactory.CreateClient("orders");

        var httpResponseMessage = await client.PutAsync($"orders/{orderId}/reserve-stock", null);

        var result = (await httpResponseMessage.Content.ReadFromJsonAsync<ReserveOrderStockResult>())!;

        return result;
    }
}
