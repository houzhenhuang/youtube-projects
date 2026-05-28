namespace Orders.Processing.Strategies;

/// <summary>
/// Federal Express： 美国快递巨头，速度快，全球覆盖广，擅长国际快递和次日达。
/// </summary>
public class FedExShippingStrategy : IShippingStrategy
{
    public ShippingProvider ProviderName => ShippingProvider.FedEx;
    public decimal CalculateCost(Order order)
    {
        return order.TotalWeight * 1.5m + 5.00m;
    }
}