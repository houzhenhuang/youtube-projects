using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Products.PublicApi;

public interface IProductService
{
    Task<ProductDto?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<ProductDto>> GetProductsByIdsAsync(int[] ids, CancellationToken cancellationToken = default);
    Task ReserveProductsAsync(int[] ids, CancellationToken cancellationToken = default);
}
