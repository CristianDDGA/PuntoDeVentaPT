using PuntoVenta.Application.DTOs.Role;

namespace PuntoVenta.Application.Interfaces.Services;

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllAsync();
    Task<RoleDto?> GetByIdAsync(int roleId);
    Task<RoleDto> CreateAsync(CreateRoleDto dto);
    Task<bool> UpdateAsync(int roleId, UpdateRoleDto dto);
    Task<bool> ActivateAsync(int roleId);
    Task<bool> DeactivateAsync(int roleId);
}