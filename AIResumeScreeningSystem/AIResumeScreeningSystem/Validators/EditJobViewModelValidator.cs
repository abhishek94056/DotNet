using AIResumeScreeningSystem.ViewModels.Job;
using FluentValidation;

namespace AIResumeScreeningSystem.Validators
{
    public class EditJobViewModelValidator : AbstractValidator<EditJobViewModel>
    {
        public EditJobViewModelValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invalid job ID.");
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Company).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).NotEmpty().MinimumLength(50);
            RuleFor(x => x.Requirements).NotEmpty();

            RuleFor(x => x.SalaryMax)
                .GreaterThanOrEqualTo(x => x.SalaryMin)
                .When(x => x.SalaryMin.HasValue && x.SalaryMax.HasValue)
                .WithMessage("Maximum salary must be >= minimum salary.");

            RuleFor(x => x.ExpiryDate)
                .GreaterThan(DateTime.Today)
                .When(x => x.ExpiryDate.HasValue)
                .WithMessage("Expiry date must be in the future.");
        }
    }
}