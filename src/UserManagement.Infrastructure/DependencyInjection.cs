
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using UserManagement.Application.Common.Interfaces;
using UserManagement.Infrastructure.Data;
using UserManagement.Infrastructure.EmailSender;
using UserManagement.Infrastructure.Redis;
using UserManagement.Infrastructure.Repositories;

namespace UserManagement.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {

        // Add services to the container.
        services.AddDbContext<ApplicationDbContext>(options =>
             options.UseSqlServer(configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IRedisCacheService>(provider => provider.GetRequiredService<RedisCacheService>());
        services.AddScoped<IEmailSender, EmailSenderService>();
        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddRedis(configuration);


  
        return services;
    }

    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)

    {
        var redisConnectionString = configuration.GetConnectionString("Redis");

        // Initialize Redis connection pool
        RedisConnectionPool.Initialize(redisConnectionString, 100);

        // Register IConnectionMultiplexer
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            return RedisConnectionPool.Instance.GetConnection();
        });

        // Register RedisCacheService
        services.AddSingleton<RedisCacheService>();
        return services;
    }

}
