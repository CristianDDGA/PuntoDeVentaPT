using System.Net.Http.Json;
using PuntoVenta.Blazor.Models;

namespace PuntoVenta.Blazor.Services;

public class DashboardApiService
{
    private readonly HttpClient _httpClient;

    public DashboardApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DashboardStatsModel?> GetStatsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<DashboardStatsModel>("api/Dashboard/stats");
        }
        catch (Exception)
        {
            // Network error or API down
            return null;
        }
    }
}
