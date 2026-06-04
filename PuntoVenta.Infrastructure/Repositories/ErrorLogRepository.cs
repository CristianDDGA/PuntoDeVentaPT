using Microsoft.EntityFrameworkCore;
using PuntoVenta.Application.Interfaces.Repositories;
using PuntoVenta.Domain.Entities;
using PuntoVenta.Infrastructure.Persistence;

namespace PuntoVenta.Infrastructure.Repositories;

public class ErrorLogRepository : IErrorLogRepository
{
    private readonly AppDbContext _appDbContext;

    public ErrorLogRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<(IEnumerable<ErrorLog> Items, int TotalCount)> SearchPagedAsync(int page, int pageSize)
    {
        var query = _appDbContext.ErrorLogs.AsNoTracking();

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(log => log.OccurredAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
