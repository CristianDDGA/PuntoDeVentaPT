using Microsoft.EntityFrameworkCore;
using PuntoVenta.Application.Interfaces.Repositories;
using PuntoVenta.Domain.Entities;
using PuntoVenta.Infrastructure.Persistence;

namespace PuntoVenta.Infrastructure.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly AppDbContext _appDbContext;

    public SaleRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<Sale>> GetAllAsync()
        => await _appDbContext.Sales
            .AsNoTracking()
            .Include(sale => sale.Customer)
            .Include(sale => sale.Details)
                .ThenInclude(saleDetail => saleDetail.Product)
            .OrderByDescending(sale => sale.SaleDate)
            .ToListAsync();

    public async Task<Sale?> GetByIdAsync(int saleId)
        => await _appDbContext.Sales
            .AsNoTracking()
            .Include(sale => sale.Customer)
            .Include(sale => sale.Details)
                .ThenInclude(saleDetail => saleDetail.Product)
            .FirstOrDefaultAsync(sale => sale.SaleId == saleId);

    public async Task<Sale?> GetByIdTrackedAsync(int saleId)
        => await _appDbContext.Sales
            .Include(sale => sale.Customer)
            .Include(sale => sale.Details)
                .ThenInclude(saleDetail => saleDetail.Product)
            .FirstOrDefaultAsync(sale => sale.SaleId == saleId);

    public async Task<Sale> AddAsync(Sale newSale)
    {
        await _appDbContext.Sales.AddAsync(newSale);
        await _appDbContext.SaveChangesAsync();
        return newSale;
    }

    public async Task UpdateAsync(Sale sale)
    {
        // If entity is untracked, we would need to attach it, but we assume it's tracked by GetByIdTrackedAsync
        if (_appDbContext.Entry(sale).State == EntityState.Detached)
        {
            _appDbContext.Sales.Update(sale);
        }
        await _appDbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<int> GetLastSaleIdAsync()
        => await _appDbContext.Sales.AnyAsync()
            ? await _appDbContext.Sales.MaxAsync(sale => sale.SaleId)
            : 0;

    /// <inheritdoc />
    public async Task<(IEnumerable<Sale> Items, int TotalCount)> SearchPagedAsync(
        int?    saleId,
        string? customerName,
        int     page,
        int     pageSize,
        bool    excludeVoided = false)
    {
        var query = _appDbContext.Sales
            .AsNoTracking()
            .Include(sale => sale.Customer)
            .Include(sale => sale.Details)
                .ThenInclude(saleDetail => saleDetail.Product)
            .AsQueryable();

        if (saleId.HasValue)
        {
            query = query.Where(sale => sale.SaleId == saleId.Value);
        }
        else if (!string.IsNullOrWhiteSpace(customerName))
        {
            query = query.Where(sale =>
                sale.Customer.FirstName.Contains(customerName) ||
                sale.Customer.LastName.Contains(customerName));
        }

        if (excludeVoided)
        {
            query = query.Where(sale => sale.Status != Domain.Enums.SaleStatus.Voided);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(sale => sale.SaleDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}