using PuntoVenta.Domain.Entities;

namespace PuntoVenta.Application.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Role?> GetByIdAsync(int roleId);
    Task<Role?> GetByNameAsync(string name);
    Task<Role> AddAsync(Role role);
    Task UpdateAsync(Role role);
    Task<bool> ActivateAsync(int roleId);
    Task<bool> DeactivateAsync(int roleId);
}