using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ApiTemplate.Application.Common.Persistence;
using ApiTemplate.Persistence.Configurations;
using ApiTemplate.Persistence.Context;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace ApiTemplate.Persistence;

public static class Startup
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<DatabaseSettings>()
            .BindConfiguration(nameof(DatabaseSettings))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddDbContext<DbContext, ApplicationDbContext>((p, optionsBuilder) =>
            {
                var databaseSettings = p.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                var loggingSettings = databaseSettings.Logging!;

                if (loggingSettings.LogToOutput)
                {
                    optionsBuilder.LogTo(m => System.Diagnostics.Debug.WriteLine(m), LogLevel.Information);
                }

                if (loggingSettings.LogSQLData)
                    optionsBuilder.EnableSensitiveDataLogging();

                optionsBuilder.UseSqlServer(
                    databaseSettings.ConnectionString,
                    e => e.MigrationsAssembly("ApiTemplate.Persistence")
                );
            })
            .AddScoped(typeof(IRepository<>), typeof(ApplicationDbRepository<>))
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<QueryFactory>(p =>
            {
                var logger = p.GetService<ILogger<QueryFactory>>();
                var databaseSettings = p.GetRequiredService<IOptions<DatabaseSettings>>().Value;

                var dbConnection = new SqlConnection(databaseSettings.ConnectionString);
                var compiler = new SqlServerCompiler();
                var queryFactory = new QueryFactory(dbConnection, compiler);
                var loggingSettings = databaseSettings.Logging;

                queryFactory.Logger = result =>
                {
                    var message = loggingSettings!.LogSQLData ? result.ToString() : result.Sql;

                    if (loggingSettings.LogToOutput)
                    {
                        System.Diagnostics.Debug.WriteLine(message);
                    }
                    else
                    {
                        logger?.LogInformation(message);
                    }
                };

                return queryFactory;
            })
            .AddScoped<IQueryRepository, QueryRepository>();

        var configurationTypes = typeof(ApplicationDbContext).Assembly.DefinedTypes
            .Where(t => !t.IsAbstract
                        && !t.IsGenericTypeDefinition
                        && typeof(EntityTypeConfigurationDependency).IsAssignableFrom(t)
            );

        foreach (var type in configurationTypes)
        {
            services.AddSingleton(typeof(EntityTypeConfigurationDependency), type);
        }

        return services;
    }
}
