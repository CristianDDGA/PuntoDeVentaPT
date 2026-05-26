using Mapster;
using PuntoVenta.Application.DTOs.Common;
using PuntoVenta.Application.DTOs.Product;
using PuntoVenta.Application.Interfaces.Repositories;
using PuntoVenta.Application.Interfaces.Services;
using PuntoVenta.Domain.Entities;

namespace PuntoVenta.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Adapt<IEnumerable<ProductDto>>();
    }

    public async Task<IEnumerable<ProductDto>> SearchByNameAsync(string name)
    {
        var matchingProducts = await _productRepository.SearchByNameAsync(name);
        return matchingProducts.Adapt<IEnumerable<ProductDto>>();
    }

    public async Task<ProductDto?> GetByIdAsync(int productId)
    {
        var existingProduct = await _productRepository.GetByIdAsync(productId);
        return existingProduct?.Adapt<ProductDto>();
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto createProductDto)
    {
        var newProduct = Product.Create(
            createProductDto.Name,
            createProductDto.Price,
            createProductDto.Stock);

        var savedProduct = await _productRepository.AddAsync(newProduct);
        return savedProduct.Adapt<ProductDto>();
    }

    public async Task<PagedResult<ProductDto>> SearchPagedAsync(
        int?    productId,
        string? name,
        int     page,
        int     pageSize)
    {
        var (items, totalCount) = await _productRepository.SearchPagedAsync(productId, name, page, pageSize);
        var productDtos         = items.Adapt<IEnumerable<ProductDto>>();
        return PagedResult<ProductDto>.Create(productDtos, totalCount, page, pageSize);
    }
}