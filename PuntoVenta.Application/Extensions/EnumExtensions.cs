using PuntoVenta.Domain.Enums;

namespace PuntoVenta.Application.Extensions;

public static class EnumExtensions
{
    public static string ToSpanish(this PaymentType paymentType)
    {
        return paymentType switch
        {
            PaymentType.Cash     => "Efectivo",
            PaymentType.Card     => "Tarjeta",
            PaymentType.Transfer => "Transferencia",
            _                    => paymentType.ToString()
        };
    }

    public static string ToSpanish(this SaleStatus status)
    {
        return status switch
        {
            SaleStatus.Pending => "Pendiente",
            SaleStatus.Paid    => "Pagado",
            SaleStatus.Voided  => "Anulado",
            _                  => status.ToString()
        };
    }
}
