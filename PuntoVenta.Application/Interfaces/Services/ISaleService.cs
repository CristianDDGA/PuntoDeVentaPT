using PuntoVenta.Application.DTOs.Common;
using PuntoVenta.Application.DTOs.Sale;

namespace PuntoVenta.Application.Interfaces.Services;

public interface ISaleService
{
    Task<IEnumerable<SaleDto>> GetAllAsync();
    Task<SaleDto?>             GetByIdAsync(int saleId);
    Task<SaleDto>              CreateAsync(CreateSaleDto dto);

    /// <summary>
    /// Returns the next correlative invoice number (last SaleId + 1).
    /// </summary>
    Task<int> GetNextInvoiceNumberAsync();

    /// <summary>
    /// Returns a paginated subset of sales filtered by optional saleId (exact) or customerName (LIKE).
    /// </summary>
    Task<PagedResult<SaleDto>> SearchPagedAsync(
        int?    saleId,
        string? customerName,
        int     page,
        int     pageSize,
        bool    excludeVoided = false);

    Task<bool> VoidSaleAsync(int saleId);
    Task<bool> MarkAsPaidAsync(int saleId);
    Task<bool> ConfirmSaleAsync(int saleId, int? userId = null);
    Task<bool> CancelSaleAsync(int saleId, int? userId = null);
}