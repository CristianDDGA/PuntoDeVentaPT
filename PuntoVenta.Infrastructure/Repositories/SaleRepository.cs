using Microsoft.EntityFrameworkCore;
using PuntoVenta.Application.DTOs.Dashboard;
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
            query = query.Where(sale => sale.Status != Domain.Enums.SaleStatus.Cancelled);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(sale => sale.SaleDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
    // 🚀 1. Conteo rápido de ventas sin cargar nada en memoria
    public async Task<int> GetTotalSalesCountAsync()
        => await _appDbContext.Sales.AsNoTracking().CountAsync();

    // 🚀 2. Solo descarga el Total y la Fecha de las 100k ventas. ¡No descarga Clientes ni Detalles!
    public async Task<IEnumerable<(decimal Total, DateTime SaleDate)>> GetSalesStatsOptimizedAsync()
    {
        var data = await _appDbContext.Sales
            .AsNoTracking()
            .Where(s => s.Status != Domain.Enums.SaleStatus.Cancelled)
            .Select(s => new { s.Total, s.SaleDate })
            .ToListAsync();

        return data.Select(x => (x.Total, x.SaleDate));
    }

    // 🚀 3. Trae estrictamente las últimas 8 ventas procesadas por Oracle
    public async Task<List<RecentSaleDto>> GetRecentSalesDashboardAsync(int count)
        => await _appDbContext.Sales
            .AsNoTracking()
            .OrderByDescending(s => s.SaleDate)
            .Take(count)
            .Select(s => new RecentSaleDto
            {
                SaleId = s.SaleId,
                CustomerName = (s.Customer.FirstName + " " + s.Customer.LastName).Trim(),
                SaleDate = s.SaleDate,
                Total = s.Total,
                Status = s.Status.ToString() // O la extensión que mapee a texto
            })
            .ToListAsync();

    // 🚀 4. El motor de Oracle procesa los 100k datos del mes y te regresa SOLO 5 filas por red
    public async Task<List<TopProductDto>> GetTopProductsDashboardAsync(int days, int topCount)
    {
        var limitDate = DateTime.Today.AddDays(-days);

        return await _appDbContext.SaleDetails
            .AsNoTracking()
            .Where(d => d.Sale.Status != Domain.Enums.SaleStatus.Cancelled && d.Sale.SaleDate >= limitDate)
            .GroupBy(d => new { d.ProductId, d.Product.Name })
            .Select(g => new TopProductDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name ?? string.Empty,
                TotalUnits = g.Sum(d => d.Quantity),
                TotalRevenue = g.Sum(d => d.Quantity * d.UnitPrice)
            })
            .OrderByDescending(p => p.TotalUnits)
            .Take(topCount)
            .ToListAsync();
    }
}