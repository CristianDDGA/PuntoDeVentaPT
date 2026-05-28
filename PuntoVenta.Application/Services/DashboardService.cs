using PuntoVenta.Application.Extensions;
using PuntoVenta.Application.DTOs.Dashboard;
using PuntoVenta.Application.Interfaces.Repositories;
using PuntoVenta.Application.Interfaces.Services;
using PuntoVenta.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PuntoVenta.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly ISaleRepository _saleRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;

    public DashboardService(
        ISaleRepository saleRepository,
        ICustomerRepository customerRepository,
        IProductRepository productRepository)
    {
        _saleRepository = saleRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
    }

    public async Task<DashboardStatsDto> GetStatsAsync()
    {
        var today = DateTime.Today;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);

        // ── 1. Conteos Globales directos y rápidos ──────────────────────────
        var totalSales = await _saleRepository.GetTotalSalesCountAsync();
        var totalCustomers = await _customerRepository.GetTotalCustomersCountAsync(); // Asegúrate de tener este método o usa un GetAll simplificado en su rep
        var totalProducts = await _productRepository.GetTotalProductsCountAsync();   // Asegúrate de tener este método o usa un GetAll simplificado en su rep

        // ── 2. Datos optimizados de ventas (Solo bajan columnas clave) ──────
        var salesStats = await _saleRepository.GetSalesStatsOptimizedAsync();

        var totalRevenueAllTime = salesStats.Sum(s => s.Total);

        var todaySalesData = salesStats.Where(s => s.SaleDate.Date == today).ToList();
        var todaySaleCount = todaySalesData.Count;
        var todayRevenue = todaySalesData.Sum(s => s.Total);

        var monthSalesData = salesStats.Where(s => s.SaleDate >= startOfMonth).ToList();
        var monthSaleCount = monthSalesData.Count;
        var monthRevenue = monthSalesData.Sum(s => s.Total);

        // ── 3. Recientes (La BD ya filtró y devolvió solo 8 registros) ──────
        var recentSales = await _saleRepository.GetRecentSalesDashboardAsync(8);

        // ── 4. Top productos (La BD ya procesó los 100k y devolvió solo 5) ──
        var topProducts = await _saleRepository.GetTopProductsDashboardAsync(30, 5);

        // ── 5. Resumen mensual (Últimos 6 meses) ───────────────────────────
        var monthlySummaries = Enumerable.Range(0, 6)
            .Select(offset =>
            {
                var referenceDate = today.AddMonths(-offset);
                var year = referenceDate.Year;
                var month = referenceDate.Month;
                var salesInMonth = salesStats.Where(s => s.SaleDate.Year == year && s.SaleDate.Month == month).ToList();

                return new MonthlySummaryDto
                {
                    Year = year,
                    Month = month,
                    MonthLabel = referenceDate.ToString("MMM yyyy"),
                    SaleCount = salesInMonth.Count,
                    Revenue = salesInMonth.Sum(s => s.Total)
                };
            })
            .OrderBy(m => m.Year).ThenBy(m => m.Month)
            .ToList();

        return new DashboardStatsDto
        {
            TotalSales = totalSales,
            TotalCustomers = totalCustomers,
            TotalProducts = totalProducts,
            TotalRevenueAllTime = totalRevenueAllTime,
            TodaySaleCount = todaySaleCount,
            TodayRevenue = todayRevenue,
            MonthSaleCount = monthSaleCount,
            MonthRevenue = monthRevenue,
            RecentSales = recentSales,
            TopProducts = topProducts,
            MonthlySummaries = monthlySummaries
        };
    }
}