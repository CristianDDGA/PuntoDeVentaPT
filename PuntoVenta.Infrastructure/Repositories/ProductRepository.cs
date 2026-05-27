using Microsoft.EntityFrameworkCore;
using PuntoVenta.Application.Interfaces.Repositories;
using PuntoVenta.Domain.Entities;
using PuntoVenta.Infrastructure.Persistence;

namespace PuntoVenta.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _appDbContext;

    public ProductRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
        => await _appDbContext.Products
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<Product>> SearchByNameAsync(string name)
        => await _appDbContext.Products
            .AsNoTracking()
            .Where(product => product.Name.Contains(name))
            .ToListAsync();

    public async Task<Product?> GetByIdAsync(int productId)
        => await _appDbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(product => product.ProductId == productId);

    public async Task<Product> AddAsync(Product newProduct)
    {
        await _appDbContext.Products.AddAsync(newProduct);
        await _appDbContext.SaveChangesAsync();
        return newProduct;
    }

    public async Task UpdateStockAsync(int productId, int newStock)
    {
        var existingProduct = await _appDbContext.Products
            .FirstOrDefaultAsync(product => product.ProductId == productId);

        if (existingProduct is null) return;

        _appDbContext.Entry(existingProduct)
            .Property(product => product.Stock)
            .CurrentValue = newStock;

        await _appDbContext.SaveChangesAsync();
    }

    public async Task<bool> ReduceStockAsync(int productId, int quantity)
    {
        // Usamos un Update atómico directo en SQL para evitar condiciones de carrera multiusuario.
        // Solo resta si el stock actual es mayor o igual a la cantidad solicitada.
        var affectedRows = await _appDbContext.Products
            .Where(p => p.ProductId == productId && p.Stock >= quantity)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Stock, p => p.Stock - quantity));

        return affectedRows > 0;
    }

    /// <inheritdoc />
    public async Task<(IEnumerable<Product> Items, int TotalCount)> SearchPagedAsync(
        int?    productId,
        string? name,
        int     page,
        int     pageSize,
        bool    onlyInStock = false)
    {
        var query = _appDbContext.Products.AsNoTracking();

        // Filtrar solo productos con stock disponible (para el modal de venta)
        if (onlyInStock)
            query = query.Where(product => product.Stock > 0);

        if (productId.HasValue)
        {
            query = query.Where(product => product.ProductId == productId.Value);
        }
        else if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(product => product.Name.Contains(name));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(product => product.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}