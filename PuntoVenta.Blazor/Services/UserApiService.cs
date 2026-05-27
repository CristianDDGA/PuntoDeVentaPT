using System.Net.Http.Json;
using PuntoVenta.Blazor.Models;

namespace PuntoVenta.Blazor.Services;

public class UserApiService
{
    private readonly HttpClient _httpClient;

    public UserApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<UserModel>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<UserModel>>("api/Users") ?? [];
    }

    public async Task<UserModel?> GetByIdAsync(int userId)
    {
        var response = await _httpClient.GetAsync($"api/Users/{userId}");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<UserModel>();
    }

    public async Task<(bool Success, string Error)> CreateAsync(CreateUserModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Users", model);
        if (response.IsSuccessStatusCode) return (true, string.Empty);
        
        var errors = await response.Content.ReadFromJsonAsync<List<string>>();
        return (false, errors?.FirstOrDefault() ?? "Error al crear el usuario.");
    }

    public async Task<(bool Success, string Error)> UpdateAsync(UpdateUserModel model)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/Users/{model.UserId}", model);
        if (response.IsSuccessStatusCode) return (true, string.Empty);
        
        var errors = await response.Content.ReadFromJsonAsync<List<string>>();
        return (false, errors?.FirstOrDefault() ?? "Error al actualizar el usuario.");
    }

    public async Task<bool> ActivateAsync(int userId)
    {
        var response = await _httpClient.PutAsync($"api/Users/{userId}/activate", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeactivateAsync(int userId)
    {
        var response = await _httpClient.PutAsync($"api/Users/{userId}/deactivate", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UnlockAsync(int userId)
    {
        var response = await _httpClient.PutAsync($"api/Users/{userId}/unlock", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<RoleModel>> GetRolesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<RoleModel>>("api/Roles") ?? [];
    }
}
