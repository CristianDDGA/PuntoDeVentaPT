using Microsoft.EntityFrameworkCore;
using PuntoVenta.Application.Interfaces.Repositories;
using PuntoVenta.Domain.Entities;
using PuntoVenta.Infrastructure.Persistence;

namespace PuntoVenta.Infrastructure.Repositories;

public class StockMovementRepository : IStockMovementRepository
{
    private readonly AppDbContext _appDbContext;

    public StockMovementRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<StockMovement> AddAsync(StockMovement stockMovement)
    {
        await _appDbContext.StockMovements.AddAsync(stockMovement);
        await _appDbContext.SaveChangesAsync();
        return stockMovement;
    }
}