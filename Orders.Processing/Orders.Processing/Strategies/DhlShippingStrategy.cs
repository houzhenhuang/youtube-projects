namespace Orders.Processing.Strategies;

/// <summary>
/// DHL Express (Deutsche Post DHL)：德国公司，国际快递最强之一，尤其擅长跨境电商、国际空运，全球覆盖非常好。
/// </summary>
public class DhlShippingStrategy : IShippingStrategy
{
    public ShippingProvider ProviderName => ShippingProvider.DHL;
    public decimal CalculateCost(Order order)
    {
        return order.TotalWeight * 2.5m + 15.0m;
    }
}