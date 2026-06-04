using FluentValidation;
using PuntoVenta.Application.DTOs.Role;

namespace PuntoVenta.Application.Validators;

public class CreateRoleValidator : AbstractValidator<CreateRoleDto>
{
    public CreateRoleValidator()
    {
        RuleFor(role => role.Name)
            .NotEmpty().WithMessage("El nombre del rol es obligatorio.")
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("El nombre del rol no puede contener solo espacios en blanco.")
            .MinimumLength(2).WithMessage("El nombre del rol debe tener al menos 2 caracteres.")
            .MaximumLength(50).WithMessage("El nombre del rol no puede superar los 50 caracteres.")
            .Matches(@"^[a-zA-Z0-9 ]+$").WithMessage("El nombre del rol solo puede contener letras, números y espacios.");
    }
}