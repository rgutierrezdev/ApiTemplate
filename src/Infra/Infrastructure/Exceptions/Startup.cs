using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ApiTemplate.Infrastructure.Exceptions;

public static class Startup
{
    public static IServiceCollection AddExceptionMiddleware(this IServiceCollection services)
    {
        services
            .AddOptions<ExceptionsSettings>()
            .BindConfiguration(nameof(ExceptionsSettings))
            .Services
            .AddScoped<ExceptionMiddleware>();

        return services;
    }

    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionMiddleware>();
    }
}
