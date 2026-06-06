using AIResumeScreeningSystem.ViewModels.Job;
using FluentValidation;

namespace AIResumeScreeningSystem.Validators
{
    public class CreateJobViewModelValidator : AbstractValidator<CreateJobViewModel>
    {
        public CreateJobViewModelValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Job title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(x => x.Company)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(200).WithMessage("Company name cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Job description is required.")
                .MinimumLength(50).WithMessage("Description must be at least 50 characters.");

            RuleFor(x => x.Requirements)
                .NotEmpty().WithMessage("Job requirements are required.");

            RuleFor(x => x.SalaryMax)
                .GreaterThanOrEqualTo(x => x.SalaryMin)
                .When(x => x.SalaryMin.HasValue && x.SalaryMax.HasValue)
                .WithMessage("Maximum salary must be greater than or equal to minimum salary.");

            RuleFor(x => x.ExpiryDate)
                .GreaterThan(DateTime.Today)
                .When(x => x.ExpiryDate.HasValue)
                .WithMessage("Expiry date must be in the future.");

            RuleFor(x => x.ExperienceYearsMax)
                .GreaterThanOrEqualTo(x => x.ExperienceYearsMin)
                .When(x => x.ExperienceYearsMax > 0)
                .WithMessage("Max experience must be >= min experience.");
        }
    }
}