namespace Orders.Processing.Strategies;

/// <summary>
/// United Parcel Service： 美国最大快递公司之一，强于地面运输（陆运）和商业快递，价格相对亲民。
/// </summary>
public class UpsShippingStrategy : IShippingStrategy
{
    public ShippingProvider ProviderName => ShippingProvider.UPS;
    public decimal CalculateCost(Order order)
    {
        return order.TotalWeight * 1.2m + 7.50m;
    }
}