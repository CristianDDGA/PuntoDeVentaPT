using FluentValidation;
using PuntoVenta.Application.DTOs.User;

namespace PuntoVenta.Application.Validators;

public class ChangeUserPasswordValidator : AbstractValidator<ChangeUserPasswordDto>
{
    public ChangeUserPasswordValidator()
    {
        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MaximumLength(200).WithMessage("La contraseña no puede superar los 200 caracteres.");
    }
}