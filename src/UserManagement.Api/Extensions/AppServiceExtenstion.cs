
using System.IO.Compression;
using Microsoft.OpenApi.Models;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using UserManagement.Infrastructure;
using UserManagement.Application;
using Polly;


using System.Text.Json;

namespace UserManagement.Api.Extensions;

public static class AppServiceExtenstion
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
    {
   
        services.AddInfrastructureServices(configuration); //  services like Dbcontext, logging, IO Services
        services.AddApplicationServices(configuration); //  services like businesslogic, validations

        // Register services to the container
        services.AddControllers(options =>
        {
            // Register the custom filter globally for all controllers
            options.Filters.Add<CustomExceptionHandler>();
        });


        services.AddAuthorization(); // Adds default authorization services
        services.AddSwagger();
        services.AddCors();
        services.AddResponseCompression();
        services.AddRateLimiter();
   
        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-XSRF-TOKEN"; // Set the header name for your custom needs
        });


        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        // Add HttpClient with Polly Retry Policy
        services.AddHttpClient("RetryClient")
            .AddTransientHttpErrorPolicy(policy =>
                policy.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));



        return services;
    }

    public static void AddResponseCompression(this IServiceCollection services)
    {
        // Add services to the container
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            // Adding selected mimetype only, since compression is already configured at IIS server.
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.
                            Concat(new[] { "application/json", "text/plain", "text/css", "application/javascript" });

            // Add the desired compression algorithms, like Gzip and Brotli
            options.Providers.Add<GzipCompressionProvider>();  // Gzip compression
            options.Providers.Add<BrotliCompressionProvider>(); // Brotli compression (better compression ratio)
                     
        });

         /*
             Fastest — Should return as fast as possible even if the result file is not optimally compressed
             NoCompression — Do nothing
             Optimal — It should do a balance between speed(the time it takes to do the compression) and the output size
             SmallestSize — It should return the smallest size, even if the compression will take a little bit longer.
        */

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });

    }

    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
        });
    }

    public static void AddCors(this IServiceCollection services)
    {
        // Add CORS policy
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins", builder =>
            {
                builder.WithOrigins("https://example.com")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
            });
        });
    }

    public static void AddRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(_ => _
            .AddFixedWindowLimiter(policyName: "fixed", options =>
            {
                options.PermitLimit = 4;
                options.Window = TimeSpan.FromSeconds(12);
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 2;
            }));

    }
}
