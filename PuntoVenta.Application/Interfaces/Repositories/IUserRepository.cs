using PuntoVenta.Domain.Entities;

namespace PuntoVenta.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int userId);
    Task<User?> GetByUsernameAsync(string username);
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> ActivateAsync(int userId);
    Task<bool> DeactivateAsync(int userId);
}