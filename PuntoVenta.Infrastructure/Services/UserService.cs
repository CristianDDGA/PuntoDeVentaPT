using Mapster;
using PuntoVenta.Application.DTOs.User;
using PuntoVenta.Application.Interfaces.Repositories;
using PuntoVenta.Application.Interfaces.Services;
using PuntoVenta.Domain.Entities;

namespace PuntoVenta.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
        => (await _userRepository.GetAllAsync()).Adapt<IEnumerable<UserDto>>();

    public async Task<UserDto?> GetByIdAsync(int userId)
        => (await _userRepository.GetByIdAsync(userId))?.Adapt<UserDto>();

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(dto.Username);
        if (existingUser is not null)
            throw new InvalidOperationException($"El usuario '{dto.Username}' ya existe.");

        var role = await _roleRepository.GetByIdAsync(dto.RoleId)
            ?? throw new KeyNotFoundException($"Rol {dto.RoleId} no encontrado.");

        var user = User.Create(
            dto.Username,
            SecurityPasswordHasher.HashPassword(dto.Password),
            dto.FullName,
            role.RoleId,
            dto.Email);

        var savedUser = await _userRepository.AddAsync(user);
        return savedUser.Adapt<UserDto>();
    }

    public async Task<bool> UpdateAsync(int userId, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null) return false;

        var role = await _roleRepository.GetByIdAsync(dto.RoleId)
            ?? throw new KeyNotFoundException($"Rol {dto.RoleId} no encontrado.");

        user.UpdateProfile(dto.FullName, dto.Email);
        user.ChangeRole(role.RoleId);
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangeUserPasswordDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null) return false;

        user.ChangePasswordHash(SecurityPasswordHasher.HashPassword(dto.Password));
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public Task<bool> ActivateAsync(int userId) => _userRepository.ActivateAsync(userId);

    public Task<bool> DeactivateAsync(int userId) => _userRepository.DeactivateAsync(userId);

    public async Task<bool> UnlockAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null) return false;

        user.UnlockUser();
        await _userRepository.UpdateAsync(user);
        return true;
    }
}