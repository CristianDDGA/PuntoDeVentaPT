using PuntoVenta.Domain.Enums;

namespace PuntoVenta.Domain.Entities;

public class StockMovement
{
    public int              StockMovementId { get; private set; }
    public int              ProductId       { get; private set; }
    public int              Quantity        { get; private set; }
    public int              PreviousStock   { get; private set; }
    public int              NewStock        { get; private set; }
    public StockMovementType MovementType   { get; private set; }
    public string?          Reference       { get; private set; }
    public DateTime         CreatedAt       { get; private set; }
    public int?             UserId          { get; private set; }

    public Product Product { get; private set; } = null!;
    public User?   User    { get; private set; }

    private StockMovement() { }

    public static StockMovement Create(
        int productId,
        int quantity,
        int previousStock,
        int newStock,
        StockMovementType movementType,
        string? reference = null,
        int? userId = null)
    {
        if (productId <= 0)
            throw new ArgumentException("El producto es obligatorio.", nameof(productId));

        if (quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(quantity));

        if (previousStock < 0 || newStock < 0)
            throw new ArgumentException("El stock no puede ser negativo.");

        return new StockMovement
        {
            ProductId = productId,
            Quantity = quantity,
            PreviousStock = previousStock,
            NewStock = newStock,
            MovementType = movementType,
            Reference = reference?.Trim(),
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };
    }
}