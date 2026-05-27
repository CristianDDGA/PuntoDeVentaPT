using FluentValidation;
using PuntoVenta.Application.DTOs.User;

namespace PuntoVenta.Application.Validators;

public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(user => user.Username)
            .NotEmpty().WithMessage("El usuario es obligatorio.")
            .MaximumLength(100).WithMessage("El usuario no puede superar los 100 caracteres.");

        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MaximumLength(200).WithMessage("La contraseña no puede superar los 200 caracteres.");

        RuleFor(user => user.FullName)
            .NotEmpty().WithMessage("El nombre completo es obligatorio.")
            .MaximumLength(150).WithMessage("El nombre completo no puede superar los 150 caracteres.");

        RuleFor(user => user.RoleId)
            .GreaterThan(0).WithMessage("Debe seleccionar un rol.");
    }
}