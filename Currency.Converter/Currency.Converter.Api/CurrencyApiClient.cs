using System.Text.Json;

namespace Currency.Converter.Api;

public class CurrencyApiClient(HttpClient httpClient, ILogger<CurrencyApiClient> logger)
{
    public async Task<decimal?> GetExchangeRateAsync(string currencyCode)
    {
        try
        {
            var response = await httpClient.GetAsync("latest");

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("API请求失败，状态为 {StatusCode}", response.StatusCode);
                return null;
            }

            var apiResposne = await response.Content.ReadFromJsonAsync<CurrencyApiResponse>(
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // 属性名称大小写不敏感
                });

            if (apiResposne?.Data != null &&
                apiResposne.Data.TryGetValue(currencyCode.ToUpperInvariant(), out var exchangeRate))
            {
                // 从Value属性中提取汇率（如果可用）
                return exchangeRate;
            }

            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取 {Currency} 的汇率时出错", currencyCode);
            return null;
        }
    }
}