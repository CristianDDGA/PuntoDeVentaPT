namespace PuntoVenta.Blazor.Models;

public class DeleteResultModel
{
    public bool   Success              { get; set; }
    public bool   IsLogicalDelete      { get; set; }
    public string Message              { get; set; } = string.Empty;
}
