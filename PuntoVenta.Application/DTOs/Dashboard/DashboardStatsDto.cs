namespace PuntoVenta.Application.DTOs.Dashboard;

/// <summary>
/// Aggregated statistics returned by the dashboard endpoint.
/// All monetary values are in the default currency.
/// </summary>
public sealed class DashboardStatsDto
{
    // ── Totals ──────────────────────────────────────────────────────────
    public int     TotalSales          { get; init; }
    public int     TotalCustomers      { get; init; }
    public int     TotalProducts       { get; init; }
    public decimal TotalRevenueAllTime { get; init; }

    // ── Today ────────────────────────────────────────────────────────────
    public int     TodaySaleCount   { get; init; }
    public decimal TodayRevenue     { get; init; }

    // ── Current month ────────────────────────────────────────────────────
    public int     MonthSaleCount   { get; init; }
    public decimal MonthRevenue     { get; init; }

    // ── Recent activity ──────────────────────────────────────────────────
    public IReadOnlyList<RecentSaleDto>      RecentSales      { get; init; } = [];
    public IReadOnlyList<TopProductDto>      TopProducts      { get; init; } = [];
    public IReadOnlyList<MonthlySummaryDto>  MonthlySummaries { get; init; } = [];
}

public sealed class RecentSaleDto
{
    public int     SaleId       { get; init; }
    public string  CustomerName { get; init; } = string.Empty;
    public DateTime SaleDate   { get; init; }
    public decimal  Total      { get; init; }
    public string   Status     { get; init; } = string.Empty;
}

public sealed class TopProductDto
{
    public int    ProductId    { get; init; }
    public string ProductName  { get; init; } = string.Empty;
    public int    TotalUnits   { get; init; }
    public decimal TotalRevenue { get; init; }
}

public sealed class MonthlySummaryDto
{
    public int     Year        { get; init; }
    public int     Month       { get; init; }
    public string  MonthLabel  { get; init; } = string.Empty;
    public int     SaleCount   { get; init; }
    public decimal Revenue     { get; init; }
}
