
using Microsoft.EntityFrameworkCore;
using UserManagement.Infrastructure.Data;

namespace UserManagement.Api.Extensions;

public static class AppMiddlewareExtension
{
    public static void AddAppMiddleware(this WebApplication app)
    {
        // Add middleware to the request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwaggerMiddleware();
        }

        app.UseCors("AllowSpecificOrigins");
        app.UseRateLimiter();
        app.UseResponseCompression();
        
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgeryMiddleware();
  
    }
    public static void UseAntiforgeryMiddleware(this IApplicationBuilder app)
    {
       // app.UseMiddleware<AntiforgeryMiddleware>();
    }
    public static void UseSwaggerMiddleware(this IApplicationBuilder app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API V1");
            c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
        });
    }
    public static async Task AddMigrationAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var environment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        if (environment.IsDevelopment())
        {
            try
            {
                // Check for pending migrations
                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    Console.WriteLine("Applying pending migrations...");
                    await dbContext.Database.MigrateAsync();
                    Console.WriteLine("Database migrations applied successfully.");
                }
                else
                {
                    Console.WriteLine("No pending migrations found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while checking or applying database migrations: {ex.Message}");
            }
        }
    }

}

