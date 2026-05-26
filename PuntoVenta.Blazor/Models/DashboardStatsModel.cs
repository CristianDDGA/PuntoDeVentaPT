namespace PuntoVenta.Blazor.Models;

/// <summary>
/// Client-side equivalent of DashboardStatsDto returned by GET /api/Dashboard/stats.
/// </summary>
public sealed class DashboardStatsModel
{
    public int     TotalSales          { get; set; }
    public int     TotalCustomers      { get; set; }
    public int     TotalProducts       { get; set; }
    public decimal TotalRevenueAllTime { get; set; }

    public int     TodaySaleCount  { get; set; }
    public decimal TodayRevenue    { get; set; }

    public int     MonthSaleCount  { get; set; }
    public decimal MonthRevenue    { get; set; }

    public List<RecentSaleModel>     RecentSales      { get; set; } = [];
    public List<TopProductModel>     TopProducts      { get; set; } = [];
    public List<MonthlySummaryModel> MonthlySummaries { get; set; } = [];
}

public sealed class RecentSaleModel
{
    public int      SaleId       { get; set; }
    public string   CustomerName { get; set; } = string.Empty;
    public DateTime SaleDate     { get; set; }
    public decimal  Total        { get; set; }
    public string   Status       { get; set; } = string.Empty;
}

public sealed class TopProductModel
{
    public int     ProductId    { get; set; }
    public string  ProductName  { get; set; } = string.Empty;
    public int     TotalUnits   { get; set; }
    public decimal TotalRevenue { get; set; }
}

public sealed class MonthlySummaryModel
{
    public int     Year       { get; set; }
    public int     Month      { get; set; }
    public string  MonthLabel { get; set; } = string.Empty;
    public int     SaleCount  { get; set; }
    public decimal Revenue    { get; set; }
}
