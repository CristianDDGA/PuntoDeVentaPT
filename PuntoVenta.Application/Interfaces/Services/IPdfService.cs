using PuntoVenta.Application.DTOs.Sale;

namespace PuntoVenta.Application.Interfaces.Services;

public interface IPdfService
{
    byte[] GenerateInvoice(SaleDto sale);
}