using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PuntoVenta.Application.DTOs.Auth;
using PuntoVenta.Application.Interfaces.Repositories;
using PuntoVenta.Application.Interfaces.Services;

namespace PuntoVenta.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
    {
        var email = loginRequestDto.Email.Trim();
        var user = await _userRepository.GetByEmailAsync(email);

        if (user is null || !user.IsActive || user.Role is null || !user.Role.IsActive)
            return null;

        if (user.IsLocked)
            throw new UnauthorizedAccessException("Su cuenta ha sido bloqueada debido a múltiples intentos fallidos. Contacte al administrador.");

        if (!SecurityPasswordHasher.VerifyPassword(loginRequestDto.Password, user.PasswordHash))
        {
            user.IncrementFailedAttempts();
            await _userRepository.UpdateAsync(user);
            return null;
        }

        user.ResetFailedAttempts();
        await _userRepository.UpdateAsync(user);

        var tokenMinutes = int.TryParse(_configuration["Security:TokenMinutes"], out var minutes)
            ? minutes
            : 480;

        var expiresAt = DateTime.UtcNow.AddMinutes(tokenMinutes);
        var token = GenerateToken(user, expiresAt);

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = user.UserId,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.Name
        };
    }

    private string GenerateToken(PuntoVenta.Domain.Entities.User user, DateTime expiresAt)
    {
        var secret = _configuration["Security:JwtSecret"] ?? "PuntoVenta_Dev_Secret_Key_2026_Must_Be_At_Least_32_Chars";
        var issuer = _configuration["Security:Issuer"] ?? "PuntoVenta.API";
        var audience = _configuration["Security:Audience"] ?? "PuntoVenta.Client";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.GivenName, user.FullName),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Role, user.Role.Name),
            new("role_id", user.RoleId.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}