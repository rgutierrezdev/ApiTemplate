using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ApiTemplate.Infrastructure.BackgroundJobs.Scheduled;
using ApiTemplate.Infrastructure.BackgroundJobs.Settings;

namespace ApiTemplate.Infrastructure.BackgroundJobs;

public static class Startup
{
    public class BackgroundJobsOptions
    {
        public bool Server { get; set; }
        public bool Scheduler { get; set; }
    }

    public class UseBackgroundJobsOptions
    {
        public bool Dashboard { get; set; }
        public bool Scheduler { get; set; }
    }

    public static IServiceCollection AddBackgroundJobs(
        this IServiceCollection services,
        IConfiguration config,
        Action<BackgroundJobsOptions>? configOptions = null
    )
    {
        var storageSettings = config.GetSection("HangfireSettings:Storage").Get<HangfireStorageSettings>();

        if (string.IsNullOrEmpty(storageSettings?.ConnectionString))
            throw new Exception("Hangfire Storage Provider ConnectionString is not configured.");

        var storageOptions = new SqlServerStorageOptions();

        if (storageSettings.Options != null)
        {
            storageOptions.QueuePollInterval = storageSettings.Options.QueuePollInterval;
            storageOptions.UseRecommendedIsolationLevel = storageSettings.Options.UseRecommendedIsolationLevel;
            storageOptions.DisableGlobalLocks = storageSettings.Options.DisableGlobalLocks;
        }

        services.AddHangfire((_, hangfireConfig) =>
            hangfireConfig.UseSqlServerStorage(storageSettings.ConnectionString, storageOptions)
        );

        if (configOptions != null)
        {
            var options = new BackgroundJobsOptions()
            {
                Server = false,
                Scheduler = false,
            };

            configOptions(options);

            if (options.Server)
            {
                var settings = config.GetSection("HangfireSettings:Server").Get<HangfireServerSettings>();

                services.AddHangfireServer(serverOptions =>
                {
                    if (settings != null)
                    {
                        serverOptions.HeartbeatInterval = settings.HeartbeatInterval;
                        serverOptions.SchedulePollingInterval = settings.SchedulePollingInterval;
                        serverOptions.ServerCheckInterval = settings.ServerCheckInterval;
                        serverOptions.WorkerCount = settings.WorkerCount;
                    }

                    serverOptions.Queues = new[]
                    {
                        "default",
                        "mailing",
                        "notification",
                    };
                });
            }

            if (options.Scheduler)
            {
                var interfaceType = typeof(IScheduledJob);

                var scheduledJobsTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(t => interfaceType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                    .Select(t => new
                    {
                        Service = t.GetInterfaces().FirstOrDefault(),
                        Implementation = t
                    })
                    .Where(t => t.Service is not null && interfaceType.IsAssignableFrom(t.Service));

                foreach (var type in scheduledJobsTypes)
                {
                    services.AddTransient(type.Service!, type.Implementation);
                }
            }
        }

        return services;
    }

    public static IApplicationBuilder UseBackgroundJobs(
        this IApplicationBuilder app,
        IConfiguration config,
        Action<UseBackgroundJobsOptions>? configOptions = null
    )
    {
        if (configOptions == null)
            return app;

        var options = new UseBackgroundJobsOptions()
        {
            Dashboard = false,
            Scheduler = false,
        };

        configOptions(options);

        if (options.Dashboard)
        {
            var settings = config.GetSection("HangfireSettings:Dashboard").Get<HangfireDashboardSettings>();

            app.UseHangfireDashboard(settings?.Route ?? "hangfire", new DashboardOptions()
            {
                StatsPollingInterval = settings?.Options?.StatsPollingInterval ?? 5000,
                DashboardTitle = settings?.Options?.DashboardTitle ?? "Hangfire Dashboard"
            });
        }

        if (options.Scheduler)
        {
            RecurringJob.AddOrUpdate<DeleteExpiredBlacklistedTokens>(
                nameof(DeleteExpiredBlacklistedTokens),
                service => service.Invoke(),
                Cron.Daily(0)
            );

            RecurringJob.AddOrUpdate<DeleteZombieFiles>(
                nameof(DeleteZombieFiles),
                service => service.Invoke(),
                Cron.Daily(0, 5)
            );
        }

        return app;
    }
}
