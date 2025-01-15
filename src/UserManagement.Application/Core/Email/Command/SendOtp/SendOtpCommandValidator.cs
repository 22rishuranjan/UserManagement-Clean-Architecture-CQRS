using FluentValidation;

namespace UserManagement.Application.Core.Email.Command.SendOtp
{
    public class SendOtpCommandValidator : AbstractValidator<SendOtpCommand>
    {
        public SendOtpCommandValidator()
        {
            RuleFor(x => x.Email)
           .NotEmpty().WithMessage("Email is required.")
           .EmailAddress().WithMessage("Invalid email format.")
           .Must(HaveValidDomain).WithMessage("Email must be from the domain '@dso.org.sg'.");
        }

        private bool HaveValidDomain(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            var domain = email.Split('@').LastOrDefault();
            return domain == "dso.org.sg";
        }
    }
}
