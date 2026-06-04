using PuntoVenta.Application.DTOs.Common;
using PuntoVenta.Application.DTOs.ErrorLog;

namespace PuntoVenta.Application.Interfaces.Services;

public interface IErrorLogService
{
    Task<PagedResult<ErrorLogDto>> GetPagedAsync(int page, int pageSize);
}
