using FluentValidation;
using NexusPay.Shared.Helpers;

namespace NexusPay.Shared.Models.Tenant.Validators
{
    public class CreateTenantRequestValidator : AbstractValidator<CreateTenantRequest>
    {
        public CreateTenantRequestValidator() 
        { 
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

            RuleFor(x => x.Document)
                .NotEmpty().WithMessage("Document is required.")
                .Must(DocumentHelper.IsValidDocument)
                .WithMessage("Document must be a valid CPF or CNPJ.");

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address");

            RuleFor(x => x.Phone)
                .Must(phone => string.IsNullOrWhiteSpace(phone) || new string(phone.Where(char.IsDigit).ToArray()).Length is 10 or 11)
                .WithMessage("Phone must be a valid Brazilian number with DDD (10 or 11 digits).")
                .MaximumLength(11).WithMessage("Phone must not exceed 11 characters.");
        }
    }
}
