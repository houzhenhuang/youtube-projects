using System.Text.Json.Serialization;

namespace Stocks.Realtime.Api.Stocks;

public class AllTickResponse
{
    [JsonPropertyName("trace")]
    public string Trace { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public AllTickData Data { get; set; } = default!;
}

public class AllTickData
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("kline_type")]
    public KlineType Type { get; set; }

    [JsonPropertyName("kline_list")]
    public List<KlineItem> KlineItems { get; set; } = [];

}

public class KlineItem
{
    /// <summary>
    /// 该K线时间戳
    /// </summary>
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    /// <summary>
    /// 该K线开盘价
    /// </summary>
    [JsonPropertyName("open_price")]
    public string OpenPrice { get; set; } = string.Empty;

    /// <summary>
    /// 该K线收盘价：
    /// 1、交易时段内，最新一根K线，该价格也是最新成交价
    /// 2、休市期间，最新一根K线，该价格是收盘价
    /// </summary>
    [JsonPropertyName("close_price")]
    public string ClosePrice { get; set; } = string.Empty;

    /// <summary>
    /// 该K线最高价
    /// </summary>
    [JsonPropertyName("high_price")]
    public string HighPrice { get; set; } = string.Empty;

    /// <summary>
    /// 该K线最低价
    /// </summary>
    [JsonPropertyName("low_price")]
    public string LowPrice { get; set; } = string.Empty;

    /// <summary>
    /// 该K线成交数量
    /// </summary>
    [JsonPropertyName("volume")]
    public string Volume { get; set; } = string.Empty;

    /// <summary>
    /// 该K线成交金额
    /// </summary>
    [JsonPropertyName("turnover")]
    public string Turnover { get; set; } = string.Empty;
}
