
namespace Stocks.Realtime.Api.Realtime;

/// <summary>
/// 股票更新选项
/// </summary>
internal sealed class StockUpdateOptions
{
    /// <summary>
    /// 更新价格间隔，默认5秒
    /// </summary>
    public TimeSpan UpdateInterval { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// 最大百分比变化
    /// </summary>
    public double MaxPercentageChange { get; set; } = 0.02; // 2%
}
