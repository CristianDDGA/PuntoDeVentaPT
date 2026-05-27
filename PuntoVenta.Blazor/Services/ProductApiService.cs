using System.Net.Http.Json;
using PuntoVenta.Blazor.Models;

namespace PuntoVenta.Blazor.Services;

public class ProductApiService
{
    private readonly HttpClient _httpClient;

    public ProductApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ProductModel>> GetAllAsync()
    {
        try
        {
            var products = await _httpClient.GetFromJsonAsync<List<ProductModel>>("api/Products");
            return products ?? [];
        }
        catch { return []; }
    }

    public async Task<ProductModel?> GetByIdAsync(int productId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ProductModel>($"api/Products/{productId}");
        }
        catch { return null; }
    }

    public async Task<List<ProductModel>> SearchByNameAsync(string name)
    {
        try
        {
            var matchingProducts = await _httpClient.GetFromJsonAsync<List<ProductModel>>(
                $"api/Products/search?name={Uri.EscapeDataString(name)}");
            return matchingProducts ?? [];
        }
        catch { return []; }
    }

    public async Task<PagedResultModel<ProductModel>?> SearchPagedAsync(
        int?    productId   = null,
        string? name        = null,
        int     page        = 1,
        int     pageSize    = 10,
        bool    onlyInStock = false)
    {
        try
        {
            var url = BuildPagedUrl("api/Products/paged", page, pageSize,
                productId.HasValue ? $"productId={productId}" : null,
                !string.IsNullOrWhiteSpace(name) ? $"name={Uri.EscapeDataString(name)}" : null,
                onlyInStock ? "onlyInStock=true" : null);

            return await _httpClient.GetFromJsonAsync<PagedResultModel<ProductModel>>(url);
        }
        catch { return null; }
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