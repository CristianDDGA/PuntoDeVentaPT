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
            _configuration["Security:DefaultAdminPassword"] ?? "Adm1n#26",
            "Administrador",
            adminRole.RoleId,
            "admin@puntoventa.local");

        await EnsureUserAsync(
            _configuration["Security:DefaultSellerUsername"] ?? "seller",
            _configuration["Security:DefaultSellerPassword"] ?? "Sell3r#2",
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
        {
            bool needsUpdate = false;
            if (string.IsNullOrWhiteSpace(existingUser.Email) && !string.IsNullOrWhiteSpace(email))
            {
                existingUser.UpdateProfile(existingUser.FullName, email);
                needsUpdate = true;
            }
            
            // Forzar actualización de la contraseña para que puedan ingresar con la nueva clave:
            existingUser.ChangePasswordHash(SecurityPasswordHasher.HashPassword(password));
            needsUpdate = true;
            
            if (needsUpdate)
            {
                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();
            }
            return;
        }

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