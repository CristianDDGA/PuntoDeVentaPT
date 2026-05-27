using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PuntoVenta.Application.Constants;
using PuntoVenta.Application.DTOs.Product;
using PuntoVenta.Application.Interfaces.Services;

namespace PuntoVenta.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Seller}")]
public class ProductsController : ControllerBase
{
    private readonly IProductService             _productService;
    private readonly IValidator<CreateProductDto> _createProductValidator;

    public ProductsController(
        IProductService              productService,
        IValidator<CreateProductDto> createProductValidator)
    {
        _productService         = productService;
        _createProductValidator = createProductValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var allProducts = await _productService.GetAllAsync();
        return Ok(allProducts);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchByName([FromQuery] string name)
    {
        var matchingProducts = await _productService.SearchByNameAsync(name);
        return Ok(matchingProducts);
    }

    /// <summary>
    /// Returns a paginated list of products.
    /// When productId is provided it takes precedence over name.
    /// </summary>
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int?    productId   = null,
        [FromQuery] string? name        = null,
        [FromQuery] int     page        = 1,
        [FromQuery] int     pageSize    = 10,
        [FromQuery] bool    onlyInStock = false)
    {
        if (page < 1)     page     = 1;
        if (pageSize < 1) pageSize = 10;

        var pagedResult = await _productService.SearchPagedAsync(productId, name, page, pageSize, onlyInStock);
        return Ok(pagedResult);
    }

    [HttpGet("{productId:int}")]
    public async Task<IActionResult> GetById(int productId)
    {
        var existingProduct = await _productService.GetByIdAsync(productId);

        if (existingProduct is null)
            return NotFound($"Product with id {productId} not found.");

        return Ok(existingProduct);
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateProductDto createProductDto)
    {
        var validationResult = await _createProductValidator.ValidateAsync(createProductDto);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

        var savedProduct = await _productService.CreateAsync(createProductDto);
        return CreatedAtAction(nameof(GetById), new { productId = savedProduct.ProductId }, savedProduct);
    }

    [HttpPut("{productId:int}/activate")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> Activate(int productId)
    {
        var success = await _productService.ActivateAsync(productId);
        return success ? NoContent() : NotFound($"Product with id {productId} not found.");
    }

    [HttpPut("{productId:int}/deactivate")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> Deactivate(int productId)
    {
        var success = await _productService.DeactivateAsync(productId);
        return success ? NoContent() : NotFound($"Product with id {productId} not found.");
    }
}