using PuntoVenta.Domain.Entities;

namespace PuntoVenta.Application.Interfaces.Repositories;

public interface IErrorLogRepository
{
    Task<(IEnumerable<ErrorLog> Items, int TotalCount)> SearchPagedAsync(int page, int pageSize);
}
