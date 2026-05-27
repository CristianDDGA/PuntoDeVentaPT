using PuntoVenta.Domain.Entities;

namespace PuntoVenta.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> SearchByNameAsync(string name);
    Task<Product?>             GetByIdAsync(int productId);
    Task<Product>              AddAsync(Product product);
    Task<bool>                 ActivateAsync(int productId);
    Task<bool>                 DeactivateAsync(int productId);
    Task                       UpdateStockAsync(int productId, int newStock);
    Task<bool>                 ReduceStockAsync(int productId, int quantity);

    /// <summary>
    /// Returns a paginated and optionally filtered list of products.
    /// Filtering by productId takes exact-match priority; name uses LIKE.
    /// </summary>
    Task<(IEnumerable<Product> Items, int TotalCount)> SearchPagedAsync(
        int?    productId,
        string? name,
        int     page,
        int     pageSize,
        bool    onlyInStock = false,
        bool    onlyActive = false);
}