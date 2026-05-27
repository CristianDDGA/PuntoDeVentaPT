using Mapster;
using PuntoVenta.Application.DTOs.Role;
using PuntoVenta.Application.Interfaces.Repositories;
using PuntoVenta.Application.Interfaces.Services;
using PuntoVenta.Domain.Entities;

namespace PuntoVenta.Infrastructure.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;

    public RoleService(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<IEnumerable<RoleDto>> GetAllAsync()
        => (await _roleRepository.GetAllAsync()).Adapt<IEnumerable<RoleDto>>();

    public async Task<RoleDto?> GetByIdAsync(int roleId)
        => (await _roleRepository.GetByIdAsync(roleId))?.Adapt<RoleDto>();

    public async Task<RoleDto> CreateAsync(CreateRoleDto dto)
    {
        var existingRole = await _roleRepository.GetByNameAsync(dto.Name);
        if (existingRole is not null)
            throw new InvalidOperationException($"El rol '{dto.Name}' ya existe.");

        var role = await _roleRepository.AddAsync(Role.Create(dto.Name));
        return role.Adapt<RoleDto>();
    }

    public async Task<bool> UpdateAsync(int roleId, UpdateRoleDto dto)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role is null) return false;

        role.Update(dto.Name);
        await _roleRepository.UpdateAsync(role);
        return true;
    }

    public Task<bool> ActivateAsync(int roleId) => _roleRepository.ActivateAsync(roleId);

    public Task<bool> DeactivateAsync(int roleId) => _roleRepository.DeactivateAsync(roleId);
}