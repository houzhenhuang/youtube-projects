using eShop.Products.PublicApi;

namespace eShop.Orders.Api.Infrastructure;

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;
    public ProductService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("products");
    }
    public async Task<ProductDto?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ProductDto>($"/api/products/{id}", cancellationToken);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<List<ProductDto>> GetProductsByIdsAsync(int[] ids, CancellationToken cancellationToken = default)
    {
        if (ids.Length == 0)
        {
            return [];
        }

        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<ProductDto>>(
                $"api/products/batch?{string.Join("&", ids.Select(id => $"ids={id}"))}", cancellationToken);

            return response ?? [];
        }
        catch (HttpRequestException ex)
        {
            return [];
        }
        catch (Exception ex) // Catch other potential exceptions like JsonException
        {
            return [];
        }
    }

    public async Task ReserveProductsAsync(int[] ids, CancellationToken cancellationToken = default)
    {
        if (ids.Length == 0)
        {
            return;
        }

        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/products/reserve",
                new { ProductIds = ids },
                cancellationToken);

            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            // Log and rethrow to allow caller to handle reservation failures
            throw;
        }
    }
}
