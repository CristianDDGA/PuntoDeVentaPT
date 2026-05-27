using FluentValidation;
using PuntoVenta.Application.DTOs.Auth;

namespace PuntoVenta.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(login => login.Username)
            .NotEmpty().WithMessage("El usuario es obligatorio.")
            .MaximumLength(100).WithMessage("El usuario no puede superar los 100 caracteres.");

        RuleFor(login => login.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MaximumLength(200).WithMessage("La contraseña no puede superar los 200 caracteres.");
    }
}