using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ApiTemplate.Application.Common.Cache;

namespace ApiTemplate.Infrastructure.Caching;

public static class Startup
{
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration config)
    {
        var settings = config.GetSection(nameof(CacheSettings)).Get<CacheSettings>();

        if (settings != null)
        {
            if (settings.UseDistributedCache)
            {
                if (settings.PreferRedis)
                {
                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = settings.RedisUrl;
                        options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions()
                        {
                            AbortOnConnectFail = true,
                            EndPoints = { settings.RedisUrl }
                        };
                    });
                }
                else
                {
                    services.AddDistributedMemoryCache();
                }

                services.AddScoped<ICacheService, DistributedCacheService>();
            }
            else
            {
                services.AddMemoryCache();
                services.AddScoped<ICacheService, LocalCacheService>();
            }
        }

        return services;
    }
}
