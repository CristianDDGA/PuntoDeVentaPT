using FluentValidation;
using PuntoVenta.Application.DTOs.User;

namespace PuntoVenta.Application.Validators;

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(user => user.FullName)
            .NotEmpty().WithMessage("El nombre completo es obligatorio.")
            .MaximumLength(150).WithMessage("El nombre completo no puede superar los 150 caracteres.");

        RuleFor(user => user.RoleId)
            .GreaterThan(0).WithMessage("Debe seleccionar un rol.");
    }
}