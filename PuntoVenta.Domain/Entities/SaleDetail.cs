using PuntoVenta.Domain.Exceptions;

namespace PuntoVenta.Domain.Entities;

public class SaleDetail
{
    public int     SaleDetailId { get; private set; }
    public int     SaleId       { get; private set; }
    public int     ProductId    { get; private set; }
    public int     Quantity     { get; private set; }
    public decimal UnitPrice    { get; private set; }
    public decimal Subtotal     => Quantity * UnitPrice;

    // Navegación
    public Product Product { get; private set; } = null!;

    private SaleDetail() { }

    public static SaleDetail Create(int productId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new DomainException("La cantidad debe ser mayor a cero.");

        if (unitPrice <= 0)
            throw new DomainException("El precio unitario debe ser mayor a cero.");

        return new SaleDetail
        {
            ProductId = productId,
            Quantity  = quantity,
            UnitPrice = unitPrice
        };
    }
}