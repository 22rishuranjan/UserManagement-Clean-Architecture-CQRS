
using Microsoft.EntityFrameworkCore;
using UserManagement.Application.Common.Exceptions;
using UserManagement.Application.Common.Interfaces;

namespace UserManagement.Application.Core.Email.Command;

public class ValidateOtpCommand : IRequest<bool>
{
    public string Email { get; set; }
    public string Code { get; set; }
}


public class ValidateOtpCommandHandler : IRequestHandler<ValidateOtpCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public ValidateOtpCommandHandler(IApplicationDbContext context)
    {
       _context = context;
    }

    public async Task<bool> Handle(ValidateOtpCommand request, CancellationToken cancellationToken)
    {
        // Find OTP for the email (in a real application, query the database)
        // Get the OTP from the database
        var otp = await _context.Otps
            .Where(o => o.Email == request.Email && o.Code == request.Code)
            .FirstOrDefaultAsync();

        if (otp?.RetryCount > 10)
        {
            throw new TooManyRetryException(otp.Email);
        }

        if (otp == null || !otp.IsValid(request.Code))
        {
            throw new OtpTimeoutException(request.Email);
        }


        return true;
    }
}
