using FluentValidation;
using MerRazvojProjekt.Server.Data;
using MerRazvojProjekt.Server.Models.Dto.CarDto;

namespace MerRazvojProjekt.Server.Validators
{
    public class CarDtoValidator : AbstractValidator<UpsertCarDto>
    {
        private readonly ApplicationDbContext dbContext;

        public CarDtoValidator(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            RuleFor(c => c.Make)
                .NotEmpty().WithMessage("Make is required")
                .MaximumLength(100).WithMessage("Make must be at most 100 characters");
            RuleFor(c => c.Model)
                .NotEmpty().WithMessage("Model is required")
                .MaximumLength(100).WithMessage("Model must be at most 100 characters");
            RuleFor(c => c.Color)
                .NotEmpty().WithMessage("Color is required")
                .MaximumLength(50).WithMessage("Color must be at most 50 characters");
            RuleFor(c => c.Year)
                .InclusiveBetween(1886, (int)DateTime.UtcNow.Year + 1)
                .WithMessage($"Year must be between 1886 and {DateTime.UtcNow.Year + 1}");
            RuleFor(c => c.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be a non-negative value");


        }
    }
}
