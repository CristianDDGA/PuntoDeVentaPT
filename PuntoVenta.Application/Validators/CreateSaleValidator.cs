using FluentValidation;
using PuntoVenta.Application.DTOs.Sale;

namespace PuntoVenta.Application.Validators;

public class CreateSaleValidator : AbstractValidator<CreateSaleDto>
{
    public CreateSaleValidator()
    {
        RuleFor(sale => sale.CustomerId)
            .GreaterThan(0).WithMessage("Debe seleccionar un cliente.");

        RuleFor(sale => sale.Details)
            .NotEmpty().WithMessage("La factura debe tener al menos un producto.");

        RuleForEach(sale => sale.Details).ChildRules(detail =>
        {
            detail.RuleFor(saleDetail => saleDetail.ProductId)
                .GreaterThan(0).WithMessage("Producto inválido.");

            detail.RuleFor(saleDetail => saleDetail.Quantity)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor a cero.");
        });
    }
}