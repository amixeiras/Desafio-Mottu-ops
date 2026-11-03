using DesafioBackend.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DesafioBackend.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMotorcycleService, MotorcycleService>();
        services.AddScoped<IDeliveryDriverService, DeliveryDriverService>();
        services.AddScoped<IRentalService, RentalService>();
        services.AddScoped<IDeliveryService, DeliveryService>();

        return services;
    }
}
