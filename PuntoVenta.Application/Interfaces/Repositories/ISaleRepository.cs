using PuntoVenta.Application.DTOs.Dashboard;
using PuntoVenta.Domain.Entities;

namespace PuntoVenta.Application.Interfaces.Repositories;

public interface ISaleRepository
{
    Task<IEnumerable<Sale>> GetAllAsync();
    Task<Sale?>             GetByIdAsync(int saleId);
    Task<Sale?>             GetByIdTrackedAsync(int saleId);
    Task<Sale>              AddAsync(Sale sale);
    Task                    UpdateAsync(Sale sale); Task<int> GetTotalSalesCountAsync();
    Task<IEnumerable<(decimal Total, DateTime SaleDate)>> GetSalesStatsOptimizedAsync();
    Task<List<RecentSaleDto>> GetRecentSalesDashboardAsync(int count);
    Task<List<TopProductDto>> GetTopProductsDashboardAsync(int days, int topCount);

    /// <summary>
    /// Returns the highest SaleId currently stored, or 0 if no sales exist.
    /// Used to compute the next correlative invoice number.
    /// </summary>
    Task<int> GetLastSaleIdAsync();

    /// <summary>
    /// Returns a paginated and optionally filtered list of sales.
    /// Filtering by saleId takes exact-match priority; customerName uses LIKE.
    /// </summary>
    Task<(IEnumerable<Sale> Items, int TotalCount)> SearchPagedAsync(
        int?    saleId,
        string? customerName,
        int     page,
        int     pageSize,
        bool    excludeVoided = false);
}