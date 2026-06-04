namespace PuntoVenta.Application.DTOs.Common;

public class DeleteResultDto
{
    public bool   Success              { get; set; }
    public bool   IsLogicalDelete      { get; set; }
    public string Message              { get; set; } = string.Empty;
}
