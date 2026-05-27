using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using PuntoVenta.Blazor.Models;

namespace PuntoVenta.Blazor.Services;

public sealed class AuthSessionService
{
    private const string SessionStorageKey = "puntoventa.auth.session";

    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    private LoginResponseModel? _currentSession;
    private bool _initialized;

    public event Action? SessionChanged;

    public AuthSessionService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        if (_initialized)
            return;

        _initialized = true;
        _currentSession = await ReadSessionAsync();
        ApplyAuthorizationHeader();
    }

    public async Task<LoginResponseModel?> LoginAsync(LoginRequestModel request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/login", request);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            string errorMessage = "Credenciales inválidas.";
            
            if (errorContent.TryGetProperty("message", out var msgElement) || 
                errorContent.TryGetProperty("Message", out msgElement))
            {
                errorMessage = msgElement.GetString() ?? errorMessage;
            }
            throw new UnauthorizedAccessException(errorMessage);
        }

        var session = await response.Content.ReadFromJsonAsync<LoginResponseModel>();

        if (session is null)
            throw new UnauthorizedAccessException("Respuesta inválida del servidor.");

        _currentSession = session;
        await StoreSessionAsync(session);
        ApplyAuthorizationHeader();
        SessionChanged?.Invoke();

        return session;
    }

    public async Task LogoutAsync()
    {
        _currentSession = null;
        ApplyAuthorizationHeader();
        await RemoveSessionAsync();
        SessionChanged?.Invoke();
    }

    public async Task<LoginResponseModel?> GetCurrentSessionAsync()
    {
        await InitializeAsync();

        if (_currentSession is null)
            return null;

        if (_currentSession.ExpiresAt <= DateTime.UtcNow)
        {
            await LogoutAsync();
            return null;
        }

        return _currentSession;
    }

    private void ApplyAuthorizationHeader()
    {
        _httpClient.DefaultRequestHeaders.Authorization = _currentSession is null
            ? null
            : new AuthenticationHeaderValue("Bearer", _currentSession.Token);
    }

    private async Task<LoginResponseModel?> ReadSessionAsync()
    {
        var serializedSession = await _jsRuntime.InvokeAsync<string?>("pvAuthStorage.get", SessionStorageKey);

        if (string.IsNullOrWhiteSpace(serializedSession))
            return null;

        return System.Text.Json.JsonSerializer.Deserialize<LoginResponseModel>(serializedSession);
    }

    private Task StoreSessionAsync(LoginResponseModel session)
    {
        var serializedSession = System.Text.Json.JsonSerializer.Serialize(session);
        return _jsRuntime.InvokeVoidAsync("pvAuthStorage.set", SessionStorageKey, serializedSession).AsTask();
    }

    private Task RemoveSessionAsync()
        => _jsRuntime.InvokeVoidAsync("pvAuthStorage.remove", SessionStorageKey).AsTask();
}