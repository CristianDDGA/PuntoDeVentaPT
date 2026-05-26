using PuntoVenta.Application.DTOs.Common;
using PuntoVenta.Application.DTOs.Product;

namespace PuntoVenta.Application.Interfaces.Services;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<IEnumerable<ProductDto>> SearchByNameAsync(string name);
    Task<ProductDto?>             GetByIdAsync(int productId);
    Task<ProductDto>              CreateAsync(CreateProductDto dto);

    /// <summary>
    /// Returns a paginated subset of products filtered by optional productId (exact) or name (LIKE).
    /// </summary>
    Task<PagedResult<ProductDto>> SearchPagedAsync(
        int?    productId,
        string? name,
        int     page,
        int     pageSize);
}