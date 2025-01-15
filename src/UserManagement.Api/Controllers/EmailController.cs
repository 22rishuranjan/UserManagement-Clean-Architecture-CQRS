using UserManagement.Application.Core.Email.Command;
using UserManagement.Application.Core.Email.Command.SendOtp;

namespace UserManagement.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class EmailController : BaseApiController
{
    private readonly IMediator _mediator;

    private readonly ILogger<EmailController> _logger;

    public EmailController(ILogger<EmailController> logger, IMediator mediator)
    {
        _mediator = mediator;
        _logger = logger;
    }


    [HttpPost("send")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpCommand otpCommand)
    {
       var result = await _mediator.Send(otpCommand);
      

        if (result)
        {
            return Ok("Email sent successfully!");
        }

        return BadRequest("Failed to send email.");
    }

    [HttpPost("validate")]
    public async Task<IActionResult> ValidateOtp([FromBody] ValidateOtpCommand otpCommand)
    {
        var result = await _mediator.Send(otpCommand);

        if (result)
        {
            return Ok("OTP validated successfully.");
        }

        return BadRequest("Failed to verify OTP.");
    }
}

