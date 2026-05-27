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
        bool    onlyInStock = false,
        bool    onlyActive  = false)
    {
        try
        {
            var url = BuildPagedUrl("api/Products/paged", page, pageSize,
                productId.HasValue ? $"productId={productId}" : null,
                !string.IsNullOrWhiteSpace(name) ? $"name={Uri.EscapeDataString(name)}" : null,
                onlyInStock ? "onlyInStock=true" : null,
                onlyActive ? "onlyActive=true" : null);

            return await _httpClient.GetFromJsonAsync<PagedResultModel<ProductModel>>(url);
        }
        catch { return null; }
    }

    public async Task<ProductModel?> CreateAsync(CreateProductModel createProductModel)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Products", createProductModel);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<ProductModel>();
        }
        catch { return null; }
    }

    public async Task<bool> ActivateAsync(int productId)
    {
        try
        {
            var response = await _httpClient.PutAsync($"api/Products/{productId}/activate", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeactivateAsync(int productId)
    {
        try
        {
            var response = await _httpClient.PutAsync($"api/Products/{productId}/deactivate", null);
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