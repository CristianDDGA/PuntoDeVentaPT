namespace PuntoVenta.Domain.Enums;

public enum SaleStatus : byte
{
    Draft     = 1,
    Confirmed = 2,
    Cancelled = 3,

    // Backward-compatible aliases for the current API/UI while we migrate flows.
    Pending = Draft,
    Paid    = Confirmed,
    Voided  = Cancelled
}
