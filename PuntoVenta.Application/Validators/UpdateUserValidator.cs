using FluentValidation;
using PuntoVenta.Application.DTOs.User;

namespace PuntoVenta.Application.Validators;

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(user => user.FullName)
            .NotEmpty().WithMessage("El nombre completo es obligatorio.")
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("El nombre no puede contener solo espacios en blanco.")
            .MaximumLength(150).WithMessage("El nombre completo no puede superar los 150 caracteres.");

        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("El formato del correo electrónico no es válido.")
            .MaximumLength(150).WithMessage("El correo electrónico no puede superar los 150 caracteres.");

        RuleFor(user => user.RoleId)
            .GreaterThan(0).WithMessage("Debe seleccionar un rol.");
    }
}