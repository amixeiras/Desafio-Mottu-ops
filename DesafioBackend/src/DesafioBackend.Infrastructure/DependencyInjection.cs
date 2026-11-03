using DesafioBackend.Application.Common.Interfaces;
using DesafioBackend.Domain.Repositories;
using DesafioBackend.Infrastructure.Messaging;
using DesafioBackend.Infrastructure.Messaging.Consumers;
using DesafioBackend.Infrastructure.Persistence;
using DesafioBackend.Infrastructure.Persistence.Repositories;
using DesafioBackend.Infrastructure.Storage;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DesafioBackend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Database - PostgreSQL
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Host=localhost;Port=5432;Database=desafiodb;Username=postgres;Password=postgrespassword";

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        // 2. Repositories and Unit of Work
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
        services.AddScoped<IDeliveryDriverRepository, DeliveryDriverRepository>();
        services.AddScoped<IRentalRepository, RentalRepository>();
        services.AddScoped<IDeliveryRepository, DeliveryRepository>();
        services.AddScoped<IMotorcycleNotificationRepository, MotorcycleNotificationRepository>();

        // 3. Storage Service
        services.AddScoped<IStorageService, LocalStorageService>();

        // 4. Messaging with MassTransit & RabbitMQ (com fallback In-Memory para testes)
        services.AddMassTransit(x =>
        {
            x.AddConsumer<MotorcycleCreatedConsumer>();

            var rabbitHost = configuration["RabbitMQ:Host"];
            if (!string.IsNullOrWhiteSpace(rabbitHost))
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitHost, "/", h =>
                    {
                        h.Username(configuration["RabbitMQ:Username"] ?? "guest");
                        h.Password(configuration["RabbitMQ:Password"] ?? "guest");
                    });

                    cfg.ConfigureEndpoints(context);
                });
            }
            else
            {
                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            }
        });

        services.AddScoped<IMessageBus, MassTransitMessageBus>();

        return services;
    }
}
