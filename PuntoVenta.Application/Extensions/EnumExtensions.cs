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
        if (status == SaleStatus.Draft || status == SaleStatus.Pending)
            return "Borrador";

        if (status == SaleStatus.Confirmed || status == SaleStatus.Paid)
            return "Confirmada";

        if (status == SaleStatus.Cancelled || status == SaleStatus.Voided)
            return "Cancelada";

        return status.ToString();
    }
}
