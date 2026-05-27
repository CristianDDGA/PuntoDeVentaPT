using Mapster;
using PuntoVenta.Application.Extensions;
using PuntoVenta.Application.DTOs.Common;
using PuntoVenta.Application.DTOs.Sale;
using PuntoVenta.Application.Interfaces.Repositories;
using PuntoVenta.Application.Interfaces.Services;
using PuntoVenta.Domain.Entities;
using PuntoVenta.Domain.Enums;

namespace PuntoVenta.Application.Services;

public class SaleService : ISaleService
{
    private readonly ISaleRepository         _saleRepository;
    private readonly ICustomerRepository     _customerRepository;
    private readonly IProductRepository       _productRepository;
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IUnitOfWork              _unitOfWork;

    public SaleService(
        ISaleRepository         saleRepository,
        ICustomerRepository     customerRepository,
        IProductRepository      productRepository,
        IStockMovementRepository stockMovementRepository,
        IUnitOfWork             unitOfWork)
    {
        _saleRepository         = saleRepository;
        _customerRepository     = customerRepository;
        _productRepository      = productRepository;
        _stockMovementRepository = stockMovementRepository;
        _unitOfWork             = unitOfWork;
    }

    public async Task<IEnumerable<SaleDto>> GetAllAsync()
    {
        var allSales = await _saleRepository.GetAllAsync();
        var dtos     = allSales.Adapt<IEnumerable<SaleDto>>().ToList();
        
        // Manual mapping for CustomerName since Mapster doesn't dig into Customer.FullName automatically
        var salesList = allSales.ToList();
        for (int i = 0; i < dtos.Count; i++)
        {
            dtos[i].CustomerName     = salesList[i].Customer?.FullName ?? string.Empty;
            dtos[i].CustomerDocument = salesList[i].Customer?.DocumentNumber ?? string.Empty;
            dtos[i].CustomerAddress  = salesList[i].Customer?.Address ?? string.Empty;
            dtos[i].CustomerCity     = salesList[i].Customer?.City ?? string.Empty;
            dtos[i].CustomerPhone    = salesList[i].Customer?.Phone ?? string.Empty;
            dtos[i].CustomerEmail    = salesList[i].Customer?.Email ?? string.Empty;
            dtos[i].PaymentType      = salesList[i].PaymentType.ToSpanish();
            dtos[i].Status           = salesList[i].Status.ToSpanish();
        }

        return dtos;
    }

    public async Task<SaleDto?> GetByIdAsync(int saleId)
    {
        var existingSale = await _saleRepository.GetByIdAsync(saleId);
        if (existingSale == null) return null;

        var dto              = existingSale.Adapt<SaleDto>();
        dto.CustomerName     = existingSale.Customer?.FullName ?? string.Empty;
        dto.CustomerDocument = existingSale.Customer?.DocumentNumber ?? string.Empty;
        dto.CustomerAddress  = existingSale.Customer?.Address ?? string.Empty;
        dto.CustomerCity     = existingSale.Customer?.City ?? string.Empty;
        dto.CustomerPhone    = existingSale.Customer?.Phone ?? string.Empty;
        dto.CustomerEmail    = existingSale.Customer?.Email ?? string.Empty;
        dto.PaymentType      = existingSale.PaymentType.ToSpanish();
        dto.Status           = existingSale.Status.ToSpanish();
        
        return dto;
    }

    public async Task<int> GetNextInvoiceNumberAsync()
    {
        var lastSaleId = await _saleRepository.GetLastSaleIdAsync();
        return lastSaleId + 1;
    }

    public async Task<SaleDto> CreateAsync(CreateSaleDto createSaleDto)
    {
        if (!Enum.TryParse<PaymentType>(createSaleDto.PaymentType, out var paymentType))
            throw new ArgumentException($"Invalid payment type: {createSaleDto.PaymentType}");

        var duplicateProductIds = createSaleDto.Details
            .GroupBy(detail => detail.ProductId)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        if (duplicateProductIds.Count > 0)
            throw new InvalidOperationException("No se permite repetir el mismo producto en el detalle de la venta.");

        var customer = await _customerRepository.GetByIdAsync(createSaleDto.CustomerId)
            ?? throw new KeyNotFoundException($"Cliente {createSaleDto.CustomerId} no encontrado.");

        if (!customer.IsActive)
            throw new InvalidOperationException("El cliente seleccionado está inactivo.");

        var saleDetails = new List<SaleDetail>();

        foreach (var saleDetailDto in createSaleDto.Details)
        {
            var existingProduct = await _productRepository.GetByIdAsync(saleDetailDto.ProductId)
                ?? throw new KeyNotFoundException($"Producto {saleDetailDto.ProductId} no encontrado.");

            if (!existingProduct.IsActive)
                throw new InvalidOperationException($"El producto {existingProduct.Name} está inactivo.");

            if (existingProduct.Stock < saleDetailDto.Quantity)
                throw new InvalidOperationException($"Stock insuficiente para el producto: {existingProduct.Name}.");

            var newSaleDetail = SaleDetail.Create(
                existingProduct.ProductId,
                saleDetailDto.Quantity,
                existingProduct.Price);

            saleDetails.Add(newSaleDetail);
        }

        var newSale   = Sale.Create(createSaleDto.CustomerId, paymentType, saleDetails);
        var savedSale = await _saleRepository.AddAsync(newSale);

        var dto              = savedSale.Adapt<SaleDto>();
        dto.CustomerName     = savedSale.Customer?.FullName ?? string.Empty;
        dto.CustomerDocument = savedSale.Customer?.DocumentNumber ?? string.Empty;
        dto.CustomerAddress  = savedSale.Customer?.Address ?? string.Empty;
        dto.CustomerCity     = savedSale.Customer?.City ?? string.Empty;
        dto.CustomerPhone    = savedSale.Customer?.Phone ?? string.Empty;
        dto.CustomerEmail    = savedSale.Customer?.Email ?? string.Empty;
        dto.PaymentType      = savedSale.PaymentType.ToSpanish();
        dto.Status           = savedSale.Status.ToSpanish();

        return dto;
    }

    public async Task<PagedResult<SaleDto>> SearchPagedAsync(
        int?    saleId,
        string? customerName,
        int     page,
        int     pageSize,
        bool    excludeVoided = false)
    {
        var (items, totalCount) = await _saleRepository.SearchPagedAsync(saleId, customerName, page, pageSize, excludeVoided);
        
        var dtos      = items.Adapt<IEnumerable<SaleDto>>().ToList();
        var itemsList = items.ToList();
        
        for (int i = 0; i < dtos.Count; i++)
        {
            dtos[i].CustomerName     = itemsList[i].Customer?.FullName ?? string.Empty;
            dtos[i].CustomerDocument = itemsList[i].Customer?.DocumentNumber ?? string.Empty;
            dtos[i].CustomerAddress  = itemsList[i].Customer?.Address ?? string.Empty;
            dtos[i].CustomerCity     = itemsList[i].Customer?.City ?? string.Empty;
            dtos[i].CustomerPhone    = itemsList[i].Customer?.Phone ?? string.Empty;
            dtos[i].CustomerEmail    = itemsList[i].Customer?.Email ?? string.Empty;
            dtos[i].PaymentType      = itemsList[i].PaymentType.ToSpanish();
            dtos[i].Status           = itemsList[i].Status.ToSpanish();
        }
        
        return PagedResult<SaleDto>.Create(dtos, totalCount, page, pageSize);
    }

    public async Task<bool> VoidSaleAsync(int saleId)
    {
        return await CancelSaleAsync(saleId);
    }

    public async Task<bool> MarkAsPaidAsync(int saleId)
    {
        return await ConfirmSaleAsync(saleId);
    }

    public async Task<bool> ConfirmSaleAsync(int saleId, int? userId = null)
    {
        var sale = await _saleRepository.GetByIdTrackedAsync(saleId);
        if (sale == null) return false;

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            if (sale.Status == SaleStatus.Cancelled)
                throw new InvalidOperationException("La factura ya se encuentra cancelada.");

            if (sale.Status == SaleStatus.Confirmed)
                throw new InvalidOperationException("La factura ya se encuentra confirmada.");

            foreach (var detail in sale.Details)
            {
                var existingProduct = await _productRepository.GetByIdAsync(detail.ProductId)
                    ?? throw new KeyNotFoundException($"Producto {detail.ProductId} no encontrado.");

                var stockReduced = await _productRepository.ReduceStockAsync(detail.ProductId, detail.Quantity);
                if (!stockReduced)
                    throw new InvalidOperationException($"Stock insuficiente para el producto: {existingProduct.Name}.");

                await _stockMovementRepository.AddAsync(StockMovement.Create(
                    detail.ProductId,
                    detail.Quantity,
                    StockMovementType.Out,
                    $"Venta #{sale.SaleId} confirmada",
                    userId));
            }

            sale.ConfirmSale();
            await _saleRepository.UpdateAsync(sale);

            await _unitOfWork.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<bool> CancelSaleAsync(int saleId, int? userId = null)
    {
        var sale = await _saleRepository.GetByIdTrackedAsync(saleId);
        if (sale == null) return false;

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            if (sale.Status == SaleStatus.Cancelled)
                throw new InvalidOperationException("La factura ya se encuentra cancelada.");

            if (sale.Status == SaleStatus.Confirmed)
            {
                foreach (var detail in sale.Details)
                {
                    var product = await _productRepository.GetByIdAsync(detail.ProductId);
                    if (product != null)
                    {
                        product.AddStock(detail.Quantity);
                        await _productRepository.UpdateStockAsync(product.ProductId, product.Stock);

                        await _stockMovementRepository.AddAsync(StockMovement.Create(
                            detail.ProductId,
                            detail.Quantity,
                            StockMovementType.In,
                            $"Venta #{sale.SaleId} cancelada",
                            userId));
                    }
                }
            }

            sale.CancelSale();
            await _saleRepository.UpdateAsync(sale);

            await _unitOfWork.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}