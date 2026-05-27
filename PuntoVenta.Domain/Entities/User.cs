namespace PuntoVenta.Domain.Entities;

public class User
{
    public int    UserId       { get; private set; }
    public string Username     { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FullName     { get; private set; } = string.Empty;
    public string? Email       { get; private set; }
    public bool   IsActive     { get; private set; } = true;
    public int    RoleId       { get; private set; }

    public Role Role { get; private set; } = null!;

    private User() { }

    public static User Create(string username, string passwordHash, string fullName, int roleId, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("El usuario es obligatorio.", nameof(username));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("La contraseña hasheada es obligatoria.", nameof(passwordHash));

        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("El nombre completo es obligatorio.", nameof(fullName));

        if (roleId <= 0)
            throw new ArgumentException("El rol es obligatorio.", nameof(roleId));

        return new User
        {
            Username = username.Trim(),
            PasswordHash = passwordHash,
            FullName = fullName.Trim(),
            RoleId = roleId,
            Email = email?.Trim()
        };
    }

    public void UpdateProfile(string fullName, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("El nombre completo es obligatorio.", nameof(fullName));

        FullName = fullName.Trim();
        Email = email?.Trim();
    }

    public void ChangePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("La contraseña hasheada es obligatoria.", nameof(passwordHash));

        PasswordHash = passwordHash;
    }

    public void ChangeRole(int roleId)
    {
        if (roleId <= 0)
            throw new ArgumentException("El rol es obligatorio.", nameof(roleId));

        RoleId = roleId;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}