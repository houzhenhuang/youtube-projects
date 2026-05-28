using System.Text.Json.Serialization;

namespace Stocks.Realtime.Api.Stocks;

public class AlphaVantageData
{
    [JsonPropertyName("Meta Data")]
    public MetaData MetaData { get; set; } = default!;

    /// <summary>
    /// 时间序列
    /// </summary>
    [JsonPropertyName("Time Series (15min)")]
    public Dictionary<string, TimeSeriesEntry> TimeSeries { get; set; } = default!;
}

public class MetaData
{
    [JsonPropertyName("1. Information")]
    public string Information { get; set; } = string.Empty;

    [JsonPropertyName("2. Symbol")]
    public string Symbol { get; set; } = string.Empty;
    
    [JsonPropertyName("3. Last Refreshed")]
    public string LastRefreshed { get; set; } = string.Empty;
    
    [JsonPropertyName("4. Interval")]
    public string Interval { get; set; } = string.Empty;

    [JsonPropertyName("5. Output Size")]
    public string OutputSize { get; set; } = string.Empty;

    [JsonPropertyName("6. Time Zone")]
    public string TimeZone { get; set; } = string.Empty;
}

/// <summary>
/// 时间序列条目
/// </summary>
public class TimeSeriesEntry
{
    [JsonPropertyName("1. open")]
    public string Open { get; set; } = string.Empty;
    
    [JsonPropertyName("2. high")]
    public string High { get; set; } = string.Empty;

    [JsonPropertyName("3. low")]
    public string Low { get; set; } = string.Empty;

    [JsonPropertyName("4. close")]
    public string Close { get; set; } = string.Empty;

    [JsonPropertyName("5. volume")]
    public string Volume { get; set; } = string.Empty;
}
