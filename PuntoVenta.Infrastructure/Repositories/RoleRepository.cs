using Microsoft.EntityFrameworkCore;
using PuntoVenta.Application.Interfaces.Repositories;
using PuntoVenta.Domain.Entities;
using PuntoVenta.Infrastructure.Persistence;

namespace PuntoVenta.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _appDbContext;

    public RoleRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
        => await _appDbContext.Roles.AsNoTracking().ToListAsync();

    public async Task<Role?> GetByIdAsync(int roleId)
        => await _appDbContext.Roles.AsNoTracking().FirstOrDefaultAsync(role => role.RoleId == roleId);

    public async Task<Role?> GetByNameAsync(string name)
        => await _appDbContext.Roles.AsNoTracking().FirstOrDefaultAsync(role => role.Name == name);

    public async Task<Role> AddAsync(Role role)
    {
        await _appDbContext.Roles.AddAsync(role);
        await _appDbContext.SaveChangesAsync();
        return role;
    }

    public async Task UpdateAsync(Role role)
    {
        await _appDbContext.Roles
            .Where(existingRole => existingRole.RoleId == role.RoleId)
            .ExecuteUpdateAsync(update => update
                .SetProperty(existingRole => existingRole.Name, role.Name)
                .SetProperty(existingRole => existingRole.IsActive, role.IsActive));
    }

    public async Task<bool> ActivateAsync(int roleId)
        => await UpdateIsActiveAsync(roleId, true);

    public async Task<bool> DeactivateAsync(int roleId)
        => await UpdateIsActiveAsync(roleId, false);

    private async Task<bool> UpdateIsActiveAsync(int roleId, bool isActive)
        => await _appDbContext.Roles.Where(role => role.RoleId == roleId)
            .ExecuteUpdateAsync(update => update.SetProperty(role => role.IsActive, isActive)) > 0;
}