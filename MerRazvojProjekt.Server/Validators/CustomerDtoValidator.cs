using FluentValidation;
using MerRazvojProjekt.Server.Data;
using MerRazvojProjekt.Server.Models.Dto.CustomerDto;

namespace MerRazvojProjekt.Server.Validators
{
    public class CustomerDtoValidator : AbstractValidator<UpsertCustomerDto>
    {
        private readonly ApplicationDbContext dbContext;

        public CustomerDtoValidator(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;

            RuleFor(c => c.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name must be at most 100 characters");

            RuleFor(c => c.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name must be at most 100 characters");

            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255);

            RuleFor(c => c.Phone)
                .MaximumLength(50).WithMessage("Phone must be at most 50 characters");

            RuleFor(c => c.City)
                .NotEmpty().WithMessage("City is required")
                .MaximumLength(100);

            RuleFor(c => c.Country)
                .NotEmpty().WithMessage("Country is required")
                .MaximumLength(100);
        }
    }
}