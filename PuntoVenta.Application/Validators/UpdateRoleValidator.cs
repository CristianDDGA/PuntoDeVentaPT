using FluentValidation;
using PuntoVenta.Application.DTOs.Role;

namespace PuntoVenta.Application.Validators;

public class UpdateRoleValidator : AbstractValidator<UpdateRoleDto>
{
    public UpdateRoleValidator()
    {
        RuleFor(role => role.Name)
            .NotEmpty().WithMessage("El nombre del rol es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre del rol no puede superar los 100 caracteres.");
    }
}