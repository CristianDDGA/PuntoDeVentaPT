using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PuntoVenta.Application.Constants;
using PuntoVenta.Domain.Entities;
using PuntoVenta.Infrastructure.Persistence;

namespace PuntoVenta.Infrastructure.Services;

public class SecuritySeedService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public SecuritySeedService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task EnsureSeededAsync()
    {
        var adminRole = await EnsureRoleAsync(AppRoles.Admin);
        var sellerRole = await EnsureRoleAsync(AppRoles.Seller);

        await EnsureUserAsync(
            _configuration["Security:DefaultAdminUsername"] ?? "admin",
            _configuration["Security:DefaultAdminPassword"] ?? "Admin123*",
            "Administrador",
            adminRole.RoleId,
            "admin@puntoventa.local");

        await EnsureUserAsync(
            _configuration["Security:DefaultSellerUsername"] ?? "seller",
            _configuration["Security:DefaultSellerPassword"] ?? "Seller123*",
            "Vendedor",
            sellerRole.RoleId,
            "seller@puntoventa.local");
    }

    private async Task<Role> EnsureRoleAsync(string roleName)
    {
        var existingRole = await _context.Roles.FirstOrDefaultAsync(role => role.Name == roleName);

        if (existingRole is not null)
            return existingRole;

        var role = Role.Create(roleName);
        await _context.Roles.AddAsync(role);
        await _context.SaveChangesAsync();

        return role;
    }

    private async Task EnsureUserAsync(string username, string password, string fullName, int roleId, string? email)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(user => user.Username == username);

        if (existingUser is not null)
            return;

        var user = User.Create(
            username,
            SecurityPasswordHasher.HashPassword(password),
            fullName,
            roleId,
            email);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}