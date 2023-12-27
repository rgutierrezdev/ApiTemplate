using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using ApiTemplate.Application.Common.Interfaces;
using ApiTemplate.Domain.Common.Exceptions;

namespace ApiTemplate.Infrastructure.Common;

public static class Startup
{
    public static IServiceCollection AddSlugifyControllers(this IServiceCollection services)
    {
        services.AddOptions<AppSettings>()
            .BindConfiguration(nameof(AppSettings));

        services.AddControllers(options =>
            {
                options.Conventions.Add(
                    new RouteTokenTransformerConvention(new SlugifyParameterTransformer())
                );
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context => throw new InvalidRequestException(
                    ErrorCodes.ValidationError,
                    "Validation failed",
                    new SerializableError(context.ModelState)
                );
            })
            .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddTypeServices(typeof(ITransientService), ServiceLifetime.Transient)
            .AddTypeServices(typeof(IScopedService), ServiceLifetime.Scoped);

        return services;
    }

    private static IServiceCollection AddTypeServices(
        this IServiceCollection services,
        Type interfaceType,
        ServiceLifetime lifetime
    )
    {
        var interfaceTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(t => interfaceType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
            .Select(t => new
            {
                Service = t.GetInterfaces().FirstOrDefault(),
                Implementation = t
            })
            .Where(t => t.Service is not null && interfaceType.IsAssignableFrom(t.Service));

        foreach (var type in interfaceTypes)
        {
            services.AddService(interfaceType, type.Service!, type.Implementation, lifetime);
        }

        return services;
    }

    private static void AddService(
        this IServiceCollection services,
        Type interfaceType,
        Type serviceType,
        Type implementationType,
        ServiceLifetime lifetime
    )
    {
        switch (lifetime)
        {
            case ServiceLifetime.Transient:
                if (interfaceType == serviceType)
                    services.AddTransient(implementationType);
                else
                    services.AddTransient(serviceType, implementationType);
                break;

            case ServiceLifetime.Scoped:
                if (interfaceType == serviceType)
                    services.AddScoped(implementationType);
                else
                    services.AddScoped(serviceType, implementationType);

                break;

            case ServiceLifetime.Singleton:
                if (interfaceType == serviceType)
                    services.AddSingleton(implementationType);
                else
                    services.AddSingleton(serviceType, implementationType);
                break;

            default:
                throw new ArgumentException("Invalid lifeTime", nameof(lifetime));
        }
    }
}
