using PuntoVenta.Domain.Entities;

namespace PuntoVenta.Application.Interfaces.Repositories;

public interface IStockMovementRepository
{
    Task<StockMovement> AddAsync(StockMovement stockMovement);
}