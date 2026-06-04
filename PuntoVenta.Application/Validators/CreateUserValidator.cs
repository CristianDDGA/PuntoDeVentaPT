using FluentValidation;
using PuntoVenta.Application.DTOs.User;

namespace PuntoVenta.Application.Validators;

public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(user => user.Username)
            .NotEmpty().WithMessage("El usuario es obligatorio.")
            .Must(username => !string.IsNullOrWhiteSpace(username)).WithMessage("El usuario no puede contener solo espacios en blanco.")
            .MinimumLength(3).WithMessage("El usuario debe tener al menos 3 caracteres.")
            .MaximumLength(50).WithMessage("El usuario no puede superar los 50 caracteres.")
            .Matches(@"^[a-zA-Z0-9._-]+$").WithMessage("El usuario solo puede contener letras, números, puntos, guiones y guiones bajos.");

        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .MaximumLength(50).WithMessage("La contraseña no puede superar los 50 caracteres.")
            .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una mayúscula.")
            .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una minúscula.")
            .Matches(@"\d").WithMessage("La contraseña debe contener al menos un número.")
            .Matches(@"[@$!%*?&]").WithMessage("La contraseña debe contener al menos un carácter especial (@$!%*?&).");

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