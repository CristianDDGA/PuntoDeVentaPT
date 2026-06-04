using System.Net.Http.Json;
using PuntoVenta.Blazor.Models;

namespace PuntoVenta.Blazor.Services;

public class ErrorLogApiService
{
    private readonly HttpClient _httpClient;

    public ErrorLogApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PagedResultModel<ErrorLogModel>?> GetPagedAsync(int page = 1, int pageSize = 15)
    {
        try
        {
            var url = $"api/ErrorLogs/paged?page={page}&pageSize={pageSize}";
            return await _httpClient.GetFromJsonAsync<PagedResultModel<ErrorLogModel>>(url);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
