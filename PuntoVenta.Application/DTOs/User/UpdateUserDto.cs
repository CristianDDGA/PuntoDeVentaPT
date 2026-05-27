namespace PuntoVenta.Application.DTOs.User;

public class UpdateUserDto
{
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int RoleId { get; set; }
}