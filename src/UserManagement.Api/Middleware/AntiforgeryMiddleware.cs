using Microsoft.AspNetCore.Antiforgery;

namespace UserManagement.Api.Middleware;

public class AntiforgeryMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IAntiforgery _antiforgery;

    public AntiforgeryMiddleware(RequestDelegate next, IAntiforgery antiforgery)
    {
        _next = next;
        _antiforgery = antiforgery;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (HttpMethods.IsPost(context.Request.Method))
        {
            try
            {
                // Validate the antiforgery token
                await _antiforgery.ValidateRequestAsync(context);
            }
            catch (AntiforgeryValidationException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Antiforgery token validation failed.");
                return;
            }
        }

        await _next(context);
    }
}


