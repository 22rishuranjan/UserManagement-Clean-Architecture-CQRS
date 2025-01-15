using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserManagement.Api.Extensions;

namespace UserManagement.Api;

public class Program
{
    public static async void Main(string[] args)
    {
        // Configure logging
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
            builder.AddEventSourceLogger(); // Optional: Event Source logger for advanced scenarios
        });
        var logger = loggerFactory.CreateLogger<Program>();

        logger.LogInformation("Starting application...");

        try
        {
            // Set minimum threads in the thread pool
            ThreadPool.SetMinThreads(1_000, 1_000);
            logger.LogInformation("Thread pool configured with minimum threads: 1,000");

            var builder = WebApplication.CreateBuilder(args);

            // Configure Kestrel server
            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.Limits.MaxConcurrentConnections = 15_000; // Test if higher limits improve throughput
                serverOptions.Limits.MaxConcurrentUpgradedConnections = 15_000;
                serverOptions.Limits.MaxRequestBodySize = 20 * 1024 * 1024; // Increase if necessary
                serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2); // Avoid premature disconnects
                serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);

                serverOptions.ListenAnyIP(5002); // Listen on port 5002 for all network interfaces

                logger.LogInformation("Kestrel server configured: MaxConcurrentConnections={MaxConcurrentConnections}, MaxConcurrentUpgradedConnections={MaxConcurrentUpgradedConnections}, MaxRequestBodySize={MaxRequestBodySize}, KeepAliveTimeout={KeepAliveTimeout}, RequestHeadersTimeout={RequestHeadersTimeout}, Listening on port 5002",
                    serverOptions.Limits.MaxConcurrentConnections,
                    serverOptions.Limits.MaxConcurrentUpgradedConnections,
                    serverOptions.Limits.MaxRequestBodySize,
                    serverOptions.Limits.KeepAliveTimeout,
                    serverOptions.Limits.RequestHeadersTimeout);
            });

            builder.AddAppLogging();
            builder.Services.AddAppServices(builder.Configuration);

            // Customise default API behaviour
            builder.Services.Configure<ApiBehaviorOptions>(options =>
                options.SuppressModelStateInvalidFilter = true);

            var app = builder.Build();
            logger.LogInformation("Application built successfully.");

            await app.AddMigrationAsync();
            logger.LogInformation("db migration applied successfully.");

            app.AddAppMiddleware();
            logger.LogInformation("Middleware configured.");

            app.MapControllers();
            logger.LogInformation("Controllers mapped.");

            logger.LogInformation("Starting the application...");
            app.Run();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "An unexpected error occurred during application startup.");
            throw;
        }
        finally
        {
            logger.LogInformation("Application shutdown complete.");
        }
    }
}
