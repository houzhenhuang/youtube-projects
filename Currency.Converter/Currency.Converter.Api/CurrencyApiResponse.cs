using System.Text.Json.Serialization;

namespace Currency.Converter.Api;

public class CurrencyApiResponse
{
    [JsonPropertyName("data")]
    public Dictionary<string, decimal> Data { get; set; } = [];
}
