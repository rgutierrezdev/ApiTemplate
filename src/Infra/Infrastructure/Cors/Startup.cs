using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiTemplate.Infrastructure.Cors;

public static class Startup
{
    private const string DefaultCorsPolicy = nameof(DefaultCorsPolicy);

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration config)
    {
        var corsSettings = config.GetSection(nameof(CorsSettings)).Get<CorsSettings>();

        if (corsSettings != null)
        {
            services.AddCors(opt =>
                opt.AddPolicy(DefaultCorsPolicy, policy =>
                    policy
                        .WithOrigins(corsSettings.Origins.ToArray())
                        .WithHeaders(corsSettings.Headers.ToArray())
                        .WithMethods(corsSettings.Methods.ToArray())
                        .AllowCredentials()
                )
            );
        }

        return services;
    }

    public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
    {
        return app.UseCors(DefaultCorsPolicy);
    }
}
