using PuntoVenta.Application.Extensions;
using PuntoVenta.Application.DTOs.Dashboard;
using PuntoVenta.Application.Interfaces.Repositories;
using PuntoVenta.Application.Interfaces.Services;
using PuntoVenta.Domain.Enums;

namespace PuntoVenta.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly ISaleRepository     _saleRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository  _productRepository;

    public DashboardService(
        ISaleRepository     saleRepository,
        ICustomerRepository customerRepository,
        IProductRepository  productRepository)
    {
        _saleRepository     = saleRepository;
        _customerRepository = customerRepository;
        _productRepository  = productRepository;
    }

    public async Task<DashboardStatsDto> GetStatsAsync()
    {
        var allSalesData = await _saleRepository.GetAllAsync();
        var allSales     = allSalesData.ToList();
        var validSales   = allSales.Where(s => s.Status != SaleStatus.Voided).ToList();
        
        var allCustomers = (await _customerRepository.GetAllAsync()).ToList();
        var allProducts  = (await _productRepository.GetAllAsync()).ToList();

        var today        = DateTime.Today;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);

        // ── Totals ──────────────────────────────────────────────────────
        var totalRevenueAllTime = validSales.Sum(s => s.Total);

        // ── Today ───────────────────────────────────────────────────────
        var todaySales    = validSales.Where(s => s.SaleDate.Date == today).ToList();
        var todayRevenue  = todaySales.Sum(s => s.Total);

        // ── Current month ────────────────────────────────────────────────
        var monthSales    = validSales.Where(s => s.SaleDate >= startOfMonth).ToList();
        var monthRevenue  = monthSales.Sum(s => s.Total);

        // ── Recent sales (last 8) ────────────────────────────────────────
        var recentSales = allSales
            .OrderByDescending(s => s.SaleDate)
            .Take(8)
            .Select(s => new RecentSaleDto
            {
                SaleId       = s.SaleId,
                CustomerName = s.Customer?.FullName ?? string.Empty,
                SaleDate     = s.SaleDate,
                Total        = s.Total,
                Status       = s.Status.ToSpanish()
            })
            .ToList();

        // ── Top products by units sold (last 30 days) ────────────────────
        var last30Days  = today.AddDays(-30);
        var topProducts = validSales
            .Where(s => s.SaleDate.Date >= last30Days)
            .SelectMany(s => s.Details)
            .GroupBy(detail => new { detail.ProductId, detail.Product?.Name })
            .Select(g => new TopProductDto
            {
                ProductId    = g.Key.ProductId,
                ProductName  = g.Key.Name ?? string.Empty,
                TotalUnits   = g.Sum(d => d.Quantity),
                TotalRevenue = g.Sum(d => d.Subtotal)
            })
            .OrderByDescending(p => p.TotalUnits)
            .Take(5)
            .ToList();

        // ── Monthly summary (last 6 months) ─────────────────────────────
        var monthlySummaries = Enumerable.Range(0, 6)
            .Select(offset =>
            {
                var referenceDate = today.AddMonths(-offset);
                var year          = referenceDate.Year;
                var month         = referenceDate.Month;
                var salesInMonth  = validSales.Where(s => s.SaleDate.Year == year && s.SaleDate.Month == month).ToList();

                return new MonthlySummaryDto
                {
                    Year       = year,
                    Month      = month,
                    MonthLabel = referenceDate.ToString("MMM yyyy"),
                    SaleCount  = salesInMonth.Count,
                    Revenue    = salesInMonth.Sum(s => s.Total)
                };
            })
            .OrderBy(m => m.Year).ThenBy(m => m.Month)
            .ToList();

        return new DashboardStatsDto
        {
            TotalSales          = allSales.Count,
            TotalCustomers      = allCustomers.Count,
            TotalProducts       = allProducts.Count,
            TotalRevenueAllTime = totalRevenueAllTime,
            TodaySaleCount      = todaySales.Count,
            TodayRevenue        = todayRevenue,
            MonthSaleCount      = monthSales.Count,
            MonthRevenue        = monthRevenue,
            RecentSales         = recentSales,
            TopProducts         = topProducts,
            MonthlySummaries    = monthlySummaries
        };
    }
}
