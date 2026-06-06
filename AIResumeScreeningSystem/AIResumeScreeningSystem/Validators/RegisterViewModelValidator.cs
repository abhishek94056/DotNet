using AIResumeScreeningSystem.ViewModels.Account;
using FluentValidation;

namespace AIResumeScreeningSystem.Validators
{
    public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
    {
        public RegisterViewModelValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("First name can only contain letters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Last name can only contain letters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Please enter a valid email address.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Please enter a valid phone number.");

            //RuleFor(x => x.Password)
            //    .NotEmpty().WithMessage("Password is required.")
            //    .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            //    .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            //    .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.");

   

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Passwords do not match.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Please select a role.")
                .Must(r => r == "Recruiter" || r == "Candidate")
                .WithMessage("Invalid role selected.");

            RuleFor(x => x.AgreeToTerms)
            .Equal(true)
            .WithMessage("You must accept the terms and conditions.");
        }
    }
}