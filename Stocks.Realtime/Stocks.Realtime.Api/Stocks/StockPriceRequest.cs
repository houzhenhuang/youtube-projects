using System.Text.Json.Serialization;

namespace Stocks.Realtime.Api.Stocks;

internal sealed record StockPriceRequest
{
    [JsonPropertyName("trace")]
    public string Trace { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public StockPriceRequestData Data { get; set; } = default!;
    public sealed record StockPriceRequestData
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("kline_type")]
        public KlineType Type { get; set; }

        /// <summary>
        /// 从指定时间往前查询K线
        /// 1、传0表示从当前最新的交易日往前查k线
        /// 2、指定时间请传时间戳，传时间戳表示从该时间戳往前查k线
        /// 3、只有外汇贵金属加密货币支持传时间戳，股票类的code不支持
        /// </summary>
        [JsonPropertyName("kline_timestamp_end")]
        public int TimestampEnd { get; set; }

        /// <summary>
        /// 1、表示查询多少根K线，每次最大请求500根，可根据时间戳循环往前请求
        /// 2、通过该字段可查询昨日收盘价，kline_type 传8，query_kline_num传2，返回2根k线数据中，时间戳较小的数据是昨日收盘价
        /// </summary>
        [JsonPropertyName("query_kline_num")]
        public int QueryKlineNum { get; set; }

        /// <summary>
        /// 复权类型,对于股票类的code才有效，例如：0:除权,1:前复权，目前仅支持0
        /// </summary>
        [JsonPropertyName("adjust_type")]
        public int AdjustType { get; set; }

    }
}

