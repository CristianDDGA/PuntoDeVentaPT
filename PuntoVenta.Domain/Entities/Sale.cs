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
    public SaleStatus  Status      { get; private set; } = SaleStatus.Draft;

    public int?        UserId      { get; private set; }

    public Customer Customer { get; private set; } = null!;
    public User?    User     { get; private set; }

    // ✅ Sin readonly para poder asignarlo en el método Create
    private List<SaleDetail> _details = [];
    public IReadOnlyList<SaleDetail> Details => _details.AsReadOnly();

    private Sale() { }

    public static Sale Create(int customerId, PaymentType paymentType, List<SaleDetail> details, int? userId = null)
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
            Status      = SaleStatus.Draft,
            UserId      = userId,
            _details    = details
        };
    }

    public void UpdateDraft(int customerId, PaymentType paymentType, List<SaleDetail> details, int? userId = null)
    {
        if (Status != SaleStatus.Draft)
            throw new DomainException("Solo se pueden modificar facturas en estado Borrador.");

        if (customerId <= 0)
            throw new DomainException("El cliente es obligatorio.");

        CustomerId  = customerId;
        PaymentType = paymentType;
        UserId      = userId;

        _details.Clear();
        if (details != null && details.Count > 0)
        {
            _details.AddRange(details);
        }

        var subtotal = _details.Sum(saleDetail => saleDetail.Subtotal);
        TaxAmount    = Math.Round(subtotal * TaxRate, 2);
        Subtotal     = subtotal;
        Total        = subtotal + TaxAmount;
    }

    public void ConfirmSale()
    {
        if (Status == SaleStatus.Confirmed)
            throw new DomainException("La factura ya se encuentra confirmada.");

        if (Status == SaleStatus.Cancelled)
            throw new DomainException("La factura ya se encuentra cancelada.");

        Status = SaleStatus.Confirmed;
    }

    public void CancelSale()
    {
        if (Status == SaleStatus.Cancelled)
            throw new DomainException("La factura ya se encuentra cancelada.");

        Status = SaleStatus.Cancelled;
    }

    public void MarkAsPaid()
    {
        ConfirmSale();
    }

    public void VoidSale()
    {
        CancelSale();
    }
}