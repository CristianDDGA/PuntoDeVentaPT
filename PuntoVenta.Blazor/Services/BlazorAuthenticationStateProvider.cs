using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace PuntoVenta.Blazor.Services;

public sealed class BlazorAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly AuthSessionService _authSessionService;

    public BlazorAuthenticationStateProvider(AuthSessionService authSessionService)
    {
        _authSessionService = authSessionService;
        _authSessionService.SessionChanged += OnSessionChanged;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var session = await _authSessionService.GetCurrentSessionAsync();

        if (session is null)
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, session.UserId.ToString()),
                new Claim(ClaimTypes.Name, session.Username),
                new Claim(ClaimTypes.GivenName, session.FullName),
                new Claim(ClaimTypes.Email, session.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, session.Role)
            },
            authenticationType: "JwtBearer");

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    private void OnSessionChanged()
        => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}