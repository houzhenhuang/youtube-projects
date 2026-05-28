namespace Orders.Processing.Strategies;

public interface IUspsApi
{
    decimal Fee();
}
public class UspsApi : IUspsApi
{
    public decimal Fee()
    {
        return 2.50m;
    }
}
/// <summary>
/// United States Postal Service：美国国家邮政，属于政府机构，价格通常最便宜，但速度较慢，常用于低价值或轻小包裹。
/// </summary>
/// <param name="uspsApi"></param>
public class UspsShippingStrategy(IUspsApi uspsApi) : IShippingStrategy
{
    public ShippingProvider ProviderName => ShippingProvider.USPS;
    public decimal CalculateCost(Order order)
    {
        return 8.99m + uspsApi.Fee();
    }
}