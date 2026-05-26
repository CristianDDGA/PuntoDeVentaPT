using PuntoVenta.Domain.Enums;
using PuntoVenta.Domain.Exceptions;

namespace PuntoVenta.Domain.Entities;

public class Sale
{
    public const decimal TaxRate = 0.12m;

    public int         SaleId      { get; private set; }
    public int         CustomerId  { get; private set; }
    public DateTime    SaleDate    { get; private set; }
    public PaymentType PaymentType { get; private set; }
    public decimal     Subtotal    { get; private set; }
    public decimal     TaxAmount   { get; private set; }
    public decimal     Total       { get; private set; }
    public SaleStatus  Status      { get; private set; } = SaleStatus.Paid;

    public Customer Customer { get; private set; } = null!;

    // ✅ Sin readonly para poder asignarlo en el método Create
    private List<SaleDetail> _details = [];
    public IReadOnlyList<SaleDetail> Details => _details.AsReadOnly();

    private Sale() { }

    public static Sale Create(int customerId, PaymentType paymentType, List<SaleDetail> details)
    {
        if (customerId <= 0)
            throw new DomainException("El cliente es obligatorio.");

        if (details == null || details.Count == 0)
            throw new DomainException("La factura debe tener al menos un producto.");

        var subtotal  = details.Sum(saleDetail => saleDetail.Subtotal);
        var taxAmount = Math.Round(subtotal * TaxRate, 2);
        var total     = subtotal + taxAmount;

        // ✅ Se asigna a través del constructor privado
        return new Sale
        {
            CustomerId  = customerId,
            PaymentType = paymentType,
            SaleDate    = DateTime.Now,
            Subtotal    = subtotal,
            TaxAmount   = taxAmount,
            Total       = total,
            Status      = SaleStatus.Paid, // Default to Paid, could be Pending later
            _details    = details
        };
    }

    public void VoidSale()
    {
        if (Status == SaleStatus.Voided)
            throw new DomainException("La factura ya se encuentra anulada.");

        Status = SaleStatus.Voided;
    }

    public void MarkAsPaid()
    {
        if (Status == SaleStatus.Paid)
            throw new DomainException("La factura ya se encuentra pagada.");
            
        Status = SaleStatus.Paid;
    }
}