using Orders.Processing.Strategies;

namespace Orders.Processing;

public class OrderProcessor
{
    private readonly IDictionary<ShippingProvider, IShippingStrategy> _shippingStrategies;

    public OrderProcessor(IEnumerable<IShippingStrategy> shippingStrategies)
    {
        _shippingStrategies = shippingStrategies.ToDictionary(s => s.ProviderName, s => s);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="order"></param>
    /// <param name="shippingProvider">运输提供商</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public decimal CalculateShippingCost(Order order, ShippingProvider shippingProvider)
    {
        if (!_shippingStrategies.TryGetValue(shippingProvider, out var strategy))
        {
            throw new ArgumentException($"Unknown shipping provider '{shippingProvider}'.", nameof(shippingProvider));
        }

        return strategy.CalculateCost(order);

    }
}