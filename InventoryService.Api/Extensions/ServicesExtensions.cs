using System.Security.Claims;
using System.Text;
using InventoryService.Application.Contracts;
using InventoryService.Application.EventBus;
using InventoryService.Application.Repositories;
using InventoryService.Infrastructure.MessageBroker;
using InventoryService.Infrastructure.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Contracts.Events;

namespace InventoryService.Api.Extensions;

/// <summary>
/// Provides extension methods for configuring services in the InventoryService API.
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Adds inventory service related services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    public static void AddInventoryServiceServices(this IServiceCollection services)
    {
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IInventoryHistoryRepository, InventoryHistoryRepository>();
    }

    /// <summary>
    /// Configures RabbitMQ and MassTransit for the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            busConfigurator.AddConsumer<ProductCreatedConsumer>();
            busConfigurator.AddConsumer<ProductUpdatedConsumer>();
            busConfigurator.AddConsumer<ProductDeletedConsumer>();
            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMQSettings = context.GetRequiredService<IOptions<RabbitMQSettings>>().Value;
                var logger = context.GetRequiredService<ILogger<RabbitMQSettings>>();
                var uri = $"amqp://{rabbitMQSettings.HostName}/";
                logger
                    .LogInformation("Connecting to RabbitMQ at {rabbitMQHostName}:{rabbitMQPort}",
                        rabbitMQSettings.HostName, rabbitMQSettings.Port);
                cfg.Host(uri, h =>
                {
                    h.Username(rabbitMQSettings.UserName);
                    h.Password(rabbitMQSettings.Password);
                });

                cfg.ReceiveEndpoint("product-notification-queue", e =>
                {
                    e.ConfigureConsumer<ProductCreatedConsumer>(context);
                    e.ConfigureConsumer<ProductDeletedConsumer>(context);
                    e.ConfigureConsumer<ProductUpdatedConsumer>(context);
                    // e.Bind("Shared.Contracts.Events.ProductCreatedEvent");
                });
            });
        });

        services.AddTransient<IEventBus, EventBus>();

        return services;
    }

    /// <summary>
    /// Configures JWT authentication and authorization policies for the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var secret = Encoding.UTF8.GetBytes(configuration["Auth:Secret"]!);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Auth:Issuer"],
                    ValidAudience = configuration["Auth:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(secret),
                    RoleClaimType = ClaimTypes.Role
                };
                options.Authority = configuration["Auth:Issuer"];
                options.RequireHttpsMetadata = false;
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly", policy => policy.RequireRole("admin"))
            .AddPolicy("AdminOrUser", policy => policy.RequireRole("admin", "user"));

        return services;
    }

    /// <summary>
    /// Configures application settings from the configuration.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddConfigSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthSettings>(configuration.GetSection("Auth"));
        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<AuthSettings>>().Value);
        services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQ"));
        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<RabbitMQSettings>>().Value);
        return services;
    }
}