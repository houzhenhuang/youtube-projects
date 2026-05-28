namespace Stocks.Realtime.Api.Stocks;

/// <summary>
/// 1、1是1分钟K，2是5分钟K，3是15分钟K，4是30分钟K，5是小时K，6是2小时K(股票不支持2小时)，7是4小时K(股票不支持4小时)，8是日K，9是周K，10是月
/// </summary>
public enum KlineType
{
    /// <summary>
    /// 1分钟K
    /// </summary>
    OneMinKline = 1,
    /// <summary>
    /// 5分钟K
    /// </summary>
    FiveMinKline = 2,
    /// <summary>
    /// 15分钟K
    /// </summary>
    FifteenKline = 3,
    /// <summary>
    /// 30分钟K
    /// </summary>
    ThirtyKline = 4,
    // <summary>
    /// 小时K
    /// </summary>
    OneHour = 5

}

