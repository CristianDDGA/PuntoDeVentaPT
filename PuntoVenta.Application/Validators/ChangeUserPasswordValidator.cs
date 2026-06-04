using FluentValidation;
using PuntoVenta.Application.DTOs.User;

namespace PuntoVenta.Application.Validators;

public class ChangeUserPasswordValidator : AbstractValidator<ChangeUserPasswordDto>
{
    public ChangeUserPasswordValidator()
    {
        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .MaximumLength(50).WithMessage("La contraseña no puede superar los 50 caracteres.")
            .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una mayúscula.")
            .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una minúscula.")
            .Matches(@"\d").WithMessage("La contraseña debe contener al menos un número.")
            .Matches(@"[@$!%*?&]").WithMessage("La contraseña debe contener al menos un carácter especial (@$!%*?&).");
    }
}