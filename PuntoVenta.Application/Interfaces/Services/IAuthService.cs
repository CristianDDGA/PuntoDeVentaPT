using PuntoVenta.Application.DTOs.Auth;

namespace PuntoVenta.Application.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequestDto);
}