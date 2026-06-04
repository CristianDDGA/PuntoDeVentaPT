using FluentValidation;
using PuntoVenta.Application.DTOs.Customer;

namespace PuntoVenta.Application.Validators;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerDto>
{
    public CreateCustomerValidator()
    {
        RuleFor(customer => customer.DocumentNumber)
            .NotEmpty().WithMessage("El número de documento (RUC/Cédula) es obligatorio.")
            .Must(doc => !string.IsNullOrWhiteSpace(doc)).WithMessage("El número de documento no puede contener solo espacios en blanco.")
            .MaximumLength(20).WithMessage("El número de documento no puede superar 20 caracteres.");

        RuleFor(customer => customer.FirstName)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .Must(name => !string.IsNullOrWhiteSpace(name)).WithMessage("El nombre no puede contener solo espacios en blanco.")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

        RuleFor(customer => customer.LastName)
            .NotEmpty().WithMessage("El apellido es obligatorio.")
            .Must(last => !string.IsNullOrWhiteSpace(last)).WithMessage("El apellido no puede contener solo espacios en blanco.")
            .MaximumLength(100).WithMessage("El apellido no puede superar 100 caracteres.");

        RuleFor(customer => customer.Email)
            .EmailAddress().WithMessage("El correo no tiene un formato válido.")
            .MaximumLength(150).WithMessage("El correo no puede superar 150 caracteres.")
            .When(customer => !string.IsNullOrWhiteSpace(customer.Email));

        RuleFor(customer => customer.Phone)
            .MaximumLength(10).WithMessage("El teléfono no puede superar 10 caracteres.")
            .Matches(@"^\d*$").WithMessage("El teléfono solo puede contener dígitos.")
            .When(customer => !string.IsNullOrWhiteSpace(customer.Phone));

        RuleFor(customer => customer.Address)
            .MaximumLength(200).WithMessage("La dirección no puede superar 200 caracteres.")
            .When(customer => !string.IsNullOrWhiteSpace(customer.Address));

        RuleFor(customer => customer.City)
            .MaximumLength(100).WithMessage("La ciudad no puede superar 100 caracteres.")
            .When(customer => !string.IsNullOrWhiteSpace(customer.City));
    }
}