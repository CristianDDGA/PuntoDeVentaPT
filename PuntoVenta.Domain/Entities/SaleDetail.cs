using PuntoVenta.Domain.Exceptions;

namespace PuntoVenta.Domain.Entities;

public class SaleDetail
{
    public int     SaleDetailId { get; private set; }
    public int     SaleId       { get; private set; }
    public int     ProductId    { get; private set; }
    public int     Quantity     { get; private set; }
    public decimal UnitPrice    { get; private set; }
    public string  ProductName  { get; private set; } = string.Empty;

    // Propiedad de navegación hacia la cabecera de la venta

    public Sale Sale { get; set; } = null!;
    public decimal Subtotal     => Quantity * UnitPrice;

    // Navegación
    public Product Product { get; private set; } = null!;

    private SaleDetail() { }

    public static SaleDetail Create(int productId, string productName, int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new DomainException("El nombre del producto es obligatorio para el registro histórico.");

        if (quantity <= 0)
            throw new DomainException("La cantidad debe ser mayor a cero.");

        if (unitPrice <= 0)
            throw new DomainException("El precio unitario debe ser mayor a cero.");

        return new SaleDetail
        {
            ProductId   = productId,
            ProductName = productName,
            Quantity    = quantity,
            UnitPrice   = unitPrice
        };
    }
}