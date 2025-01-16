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

namespace InventoryService.Api.Extensions;

public static class ServicesExtensions
{
    public static void AddInventoryServiceServices(this IServiceCollection services)
    {
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IInventoryHistoryRepository, InventoryHistoryRepository>();
    }
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQ"));
        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<RabbitMQSettings>>().Value);
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            busConfigurator.AddConsumer<ProductCreatedConsumer>();
            busConfigurator.AddConsumer<ProductUpdatedConsumer>();
            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMQSettings = context.GetRequiredService<IOptions<RabbitMQSettings>>().Value;
                cfg.Host(new Uri($"amqp://{rabbitMQSettings.HostName}:{rabbitMQSettings.Port}"), h =>
                {
                    h.Username(rabbitMQSettings.UserName);
                    h.Password(rabbitMQSettings.Password);
                });
            });
        });

        services.AddTransient<IEventBus, EventBus>();

        return services;
    }

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
                    IssuerSigningKey = new SymmetricSecurityKey(secret)
                };
                options.Authority = configuration["Auth:Issuer"];
                options.RequireHttpsMetadata = true;
            });

        return services;
    }

    public static IServiceCollection AddConfigSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthSettings>(configuration.GetSection("Auth"));
        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<AuthSettings>>().Value);
        return services;
    }
}