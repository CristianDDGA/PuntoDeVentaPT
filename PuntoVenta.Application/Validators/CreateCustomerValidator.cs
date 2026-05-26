using FluentValidation;
using PuntoVenta.Application.DTOs.Customer;

namespace PuntoVenta.Application.Validators;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerDto>
{
    public CreateCustomerValidator()
    {
        RuleFor(customer => customer.DocumentNumber)
            .NotEmpty().WithMessage("El número de documento (RUC/Cédula) es obligatorio.")
            .MaximumLength(20).WithMessage("El número de documento no puede superar 20 caracteres.");

        RuleFor(customer => customer.FirstName)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

        RuleFor(customer => customer.LastName)
            .NotEmpty().WithMessage("El apellido es obligatorio.")
            .MaximumLength(100).WithMessage("El apellido no puede superar 100 caracteres.");

        RuleFor(customer => customer.Email)
            .EmailAddress().WithMessage("El correo no tiene un formato válido.")
            .When(customer => !string.IsNullOrWhiteSpace(customer.Email));
    }
}