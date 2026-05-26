using System.Net.Http.Json;
using PuntoVenta.Blazor.Models;

namespace PuntoVenta.Blazor.Services;

public class SaleApiService
{
    private readonly HttpClient _httpClient;

    public SaleApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<SaleModel>> GetAllAsync()
    {
        try
        {
            var allSales = await _httpClient.GetFromJsonAsync<List<SaleModel>>("api/Sales");
            return allSales ?? [];
        }
        catch { return []; }
    }

    public async Task<SaleModel?> GetByIdAsync(int saleId)
    {
        try { return await _httpClient.GetFromJsonAsync<SaleModel>($"api/Sales/{saleId}"); }
        catch { return null; }
    }

    public async Task<int> GetNextInvoiceNumberAsync()
    {
        try { return await _httpClient.GetFromJsonAsync<int>("api/Sales/next-number"); }
        catch { return 0; }
    }

    public string LastErrorMessage { get; private set; } = string.Empty;

    public async Task<SaleModel?> CreateAsync(CreateSaleModel createSaleModel)
    {
        LastErrorMessage = string.Empty;
        try
        {
            var httpResponse = await _httpClient.PostAsJsonAsync("api/Sales", createSaleModel);
            
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorData = await httpResponse.Content.ReadFromJsonAsync<ErrorResponse>();
                LastErrorMessage = errorData?.Message ?? "Error desconocido al procesar la venta.";
                return null;
            }

            return await httpResponse.Content.ReadFromJsonAsync<SaleModel>();
        }
        catch (Exception ex)
        {
            LastErrorMessage = $"Error de conexión: {ex.Message}";
            return null;
        }
    }

    private class ErrorResponse { public string Message { get; set; } = string.Empty; }

    public async Task<byte[]?> GetPdfAsync(int saleId)
    {
        try
        {
            var httpResponse = await _httpClient.GetAsync($"api/Sales/{saleId}/pdf");
            if (!httpResponse.IsSuccessStatusCode) return null;
            return await httpResponse.Content.ReadAsByteArrayAsync();
        }
        catch { return null; }
    }

    public async Task<PagedResultModel<SaleModel>?> SearchPagedAsync(
        int?    saleId       = null,
        string? customerName = null,
        int     page         = 1,
        int     pageSize     = 15,
        bool    excludeVoided = false)
    {
        try
        {
            var url = BuildPagedUrl("api/Sales/paged", page, pageSize,
                saleId.HasValue ? $"saleId={saleId}" : null,
                !string.IsNullOrWhiteSpace(customerName) ? $"customerName={Uri.EscapeDataString(customerName)}" : null,
                excludeVoided ? "excludeVoided=true" : null);

            return await _httpClient.GetFromJsonAsync<PagedResultModel<SaleModel>>(url);
        }
        catch { return null; }
    }

    public async Task<bool> VoidSaleAsync(int saleId)
    {
        try
        {
            var response = await _httpClient.PutAsync($"api/Sales/{saleId}/void", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> MarkAsPaidAsync(int saleId)
    {
        try
        {
            var response = await _httpClient.PutAsync($"api/Sales/{saleId}/pay", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private static string BuildPagedUrl(string baseUrl, int page, int pageSize, params string?[] filters)
    {
        var queryParts = filters
            .Where(f => f is not null)
            .Concat([$"page={page}", $"pageSize={pageSize}"]);

        return $"{baseUrl}?{string.Join('&', queryParts)}";
    }
}