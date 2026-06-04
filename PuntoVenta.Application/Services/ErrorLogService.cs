using Mapster;
using PuntoVenta.Application.DTOs.Common;
using PuntoVenta.Application.DTOs.ErrorLog;
using PuntoVenta.Application.Interfaces.Repositories;
using PuntoVenta.Application.Interfaces.Services;

namespace PuntoVenta.Application.Services;

public class ErrorLogService : IErrorLogService
{
    private readonly IErrorLogRepository _errorLogRepository;

    public ErrorLogService(IErrorLogRepository errorLogRepository)
    {
        _errorLogRepository = errorLogRepository;
    }

    public async Task<PagedResult<ErrorLogDto>> GetPagedAsync(int page, int pageSize)
    {
        var (items, totalCount) = await _errorLogRepository.SearchPagedAsync(page, pageSize);
        var dtos = items.Adapt<IEnumerable<ErrorLogDto>>();
        return PagedResult<ErrorLogDto>.Create(dtos, totalCount, page, pageSize);
    }
}
