using FluentValidation;
using PuntoVenta.Application.DTOs.Product;

namespace PuntoVenta.Application.Validators;

public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("El nombre del producto es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no debe exceder los 100 caracteres.");

        RuleFor(p => p.Price)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a cero.");

        RuleFor(p => p.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo.");
    }
}
