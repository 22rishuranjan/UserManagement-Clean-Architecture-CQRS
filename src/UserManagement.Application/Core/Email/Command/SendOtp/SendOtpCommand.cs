using System.Security.Cryptography.X509Certificates;
using UserManagement.Application.Common.Interfaces;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Core.Email.Command.SendOtp;

public class SendOtpCommand : IRequest<bool>
{
    public string Email { get; set; }
}


public class SendOtpCommandHandler : IRequestHandler<SendOtpCommand, bool>
{
    private readonly IEmailSender _emailSender;
    private readonly IApplicationDbContext _context;
    private const string SUBJECT = "OTP for User Management APP";
    private const string BODY = "Your OTP code is {0}. It is valid for 1 minutes.";

    public SendOtpCommandHandler(IEmailSender emailSender, IApplicationDbContext context)
    {
        _emailSender = emailSender;
        _context = context;
    }

    public async Task<bool> Handle(SendOtpCommand request, CancellationToken cancellationToken)
    {
        // Generate a random OTP
        var otpCode = new Random().Next(100000, 999999).ToString();
        var expiryTime = DateTime.UtcNow.AddMinutes(1);

        // Create otp entity
        var newOtp = new Otp
        {
            Email = request.Email,  
            ExpiryTime = expiryTime,
            Code = otpCode
        };

        // Save to the in-memory DB
        _context.Otps.Add(newOtp);
        await _context.SaveChangesAsync(cancellationToken);

        // Send OTP via email
        await _emailSender.SendEmailAsync(request.Email, SUBJECT, string.Format(BODY, otpCode));

        // Log or store the OTP (in a real application, save it to the database)
        Console.WriteLine($"OTP {otpCode} sent to {request.Email}");

        return true;

    }

}
