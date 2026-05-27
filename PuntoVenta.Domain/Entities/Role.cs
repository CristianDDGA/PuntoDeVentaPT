namespace PuntoVenta.Domain.Entities;

public class Role
{
    public int    RoleId   { get; private set; }
    public string Name     { get; private set; } = string.Empty;
    public bool   IsActive { get; private set; } = true;

    private Role() { }

    public static Role Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del rol es obligatorio.", nameof(name));

        return new Role
        {
            Name = name.Trim()
        };
    }

    public void Update(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del rol es obligatorio.", nameof(name));

        Name = name.Trim();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}