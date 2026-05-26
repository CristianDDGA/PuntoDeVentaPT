using PuntoVenta.Application.DTOs.Dashboard;

namespace PuntoVenta.Application.Interfaces.Services;

public interface IDashboardService
{
    /// <summary>
    /// Computes and returns all aggregated statistics for the dashboard.
    /// </summary>
    Task<DashboardStatsDto> GetStatsAsync();
}
