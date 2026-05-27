namespace PuntoVenta.Blazor.Models;

public class ProductModel
{
    public int     ProductId { get; set; }
    public string  Name      { get; set; } = string.Empty;
    public decimal Price     { get; set; }
    public int     Stock     { get; set; }
    public bool    IsActive  { get; set; }
}

public class CreateProductModel
{
    public string  Name  { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int     Stock { get; set; }
}