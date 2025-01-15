
using FluentValidation;

namespace UserManagement.Application.Core.Email.Command
{
    public class ValidateOtpCommandValidator : AbstractValidator<ValidateOtpCommand>
    {
        public ValidateOtpCommandValidator()
        {
            RuleFor(x => x.Email)
           .NotEmpty().WithMessage("Email is required.")
           .EmailAddress().WithMessage("Invalid email format.")
           .Must(HaveValidDomain).WithMessage("Email must be from the domain '@dso.org.sg'.");

            RuleFor(x => x.Code)
           .Length(6).WithMessage("OTP must be of 6 digit");
        }

        private bool HaveValidDomain(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            var domain = email.Split('@').LastOrDefault();
            return domain == "dso.org.sg";
        }
    }
}
