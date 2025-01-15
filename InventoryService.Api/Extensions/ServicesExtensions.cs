using InventoryService.Application.Contracts;
using InventoryService.Application.EventBus;
using InventoryService.Infrastructure.MessageBroker;
using MassTransit;
using Microsoft.Extensions.Options;

namespace InventoryService.Api.Extensions;

public static class ServicesExtensions
{
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
}