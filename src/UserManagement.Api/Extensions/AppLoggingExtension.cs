using Serilog;
namespace UserManagement.Api.Extensions;

public static class AppLoggingExtension
{
    public static void AddAppLogging(this WebApplicationBuilder builder)
    {

        // Configure Serilog
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext());
        // Configure logging
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(); // Add console logging
        builder.Logging.AddDebug(); // Add debug logging (useful in development)
    }
}