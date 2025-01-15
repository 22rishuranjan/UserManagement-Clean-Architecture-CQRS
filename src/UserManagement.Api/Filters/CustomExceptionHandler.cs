using Microsoft.AspNetCore.Mvc.Filters;
using UserManagement.Application.Common.Exceptions;

public class CustomExceptionHandler : ExceptionFilterAttribute
{
    private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;
    private readonly ILogger<CustomExceptionHandler> _logger;

    public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
    {
        _logger = logger;

        // Register known exception types and handlers
        _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>()
        {
            { typeof(OtpTimeoutException), HandleOtpTimeoutException },
            { typeof(TooManyRetryException), HandleTooManyRetryException },
            { typeof(ValidationException), HandleValidationException },
            { typeof(NotFoundException), HandleNotFoundException }
        };
    }

    public override void OnException(ExceptionContext context)
    {
        // Log the exception details with additional request context
        _logger.LogError(context.Exception, "Unhandled exception occurred during {Method} {Path}.",
            context.HttpContext.Request.Method,
            context.HttpContext.Request.Path);

        HandleException(context);

        base.OnException(context);
    }

    private void HandleException(ExceptionContext context)
    {
        var type = context.Exception.GetType();

        if (_exceptionHandlers.ContainsKey(type))
        {
            _exceptionHandlers[type].Invoke(context);
            return;
        }

        // Default handler for unhandled exceptions
        var detail = new ProblemDetails()
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = "An unexpected error occurred.",
            Detail = context.Exception.Message,
            Status = StatusCodes.Status500InternalServerError
        };

        _logger.LogError("Unhandled exception: {Message}, StackTrace: {StackTrace}",
            context.Exception.Message,
            context.Exception.StackTrace);

        context.Result = new ObjectResult(detail)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
        context.ExceptionHandled = true;
    }

    private void HandleValidationException(ExceptionContext context)
    {
        var exception = (ValidationException)context.Exception;

        var detail = new ValidationProblemDetails(exception.Errors)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Validation failed."
        };

        _logger.LogWarning("ValidationException: {Errors}", exception.Errors);
        context.Result = new BadRequestObjectResult(detail);
        context.ExceptionHandled = true;
    }

    private void HandleNotFoundException(ExceptionContext context)
    {
        var exception = (NotFoundException)context.Exception;

        var detail = new ProblemDetails()
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "Resource not found.",
            Detail = exception.Message,
            Status = StatusCodes.Status404NotFound
        };

        _logger.LogWarning("NotFoundException: {Message}", exception.Message);
        context.Result = new NotFoundObjectResult(detail);
        context.ExceptionHandled = true;
    }

    private void HandleTooManyRetryException(ExceptionContext context)
    {
        var exception = (TooManyRetryException)context.Exception;

        var detail = new ProblemDetails()
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Too many retries.",
            Detail = exception.Message,
            Status = StatusCodes.Status429TooManyRequests
        };

        _logger.LogWarning("TooManyRetryException: {Message}", exception.Message);
        context.Result = new ObjectResult(detail) { StatusCode = StatusCodes.Status429TooManyRequests };
        context.ExceptionHandled = true;
    }

    private void HandleOtpTimeoutException(ExceptionContext context)
    {
        var exception = (OtpTimeoutException)context.Exception;

        var detail = new ProblemDetails()
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "OTP timeout.",
            Detail = exception.Message,
            Status = StatusCodes.Status408RequestTimeout
        };

        _logger.LogWarning("OtpTimeoutException: {Message}", exception.Message);
        context.Result = new ObjectResult(detail) { StatusCode = StatusCodes.Status408RequestTimeout };
        context.ExceptionHandled = true;
    }
}
    