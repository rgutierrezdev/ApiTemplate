using System.Reflection;
using MediatR;
using ApiTemplate.Application;
using ApiTemplate.Infrastructure.Auth;
using ApiTemplate.Infrastructure.BackgroundJobs;
using ApiTemplate.Infrastructure.Caching;
using ApiTemplate.Infrastructure.Common;
using ApiTemplate.Infrastructure.Cors;
using ApiTemplate.Infrastructure.Exceptions;
using ApiTemplate.Infrastructure.Mailing;
using ApiTemplate.Infrastructure.OpenAPI;
using ApiTemplate.Infrastructure.Storage;
using ApiTemplate.Persistence;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var services = builder.Services;

config
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

services
    .AddSlugifyControllers()
    .AddHttpContextAccessor();

QuestPDF.Settings.License = LicenseType.Community;

// Add Infra Services
services
    .AddPersistence(config)
    .AddAuth(config)
    .AddBackgroundJobs(config, options =>
    {
        options.Server = true;
        options.Scheduler = true;
    })
    .AddCaching(config)
    .AddCorsPolicy(config)
    .AddExceptionMiddleware()
    .AddStorage(config)
    .AddMailing(config)
    .AddMediatR(Assembly.GetExecutingAssembly())
    .AddOpenApiDocumentation(config)
    .AddRouting(options => options.LowercaseUrls = true)
    .AddServices();

// Add Application Services
services.AddApplication();

var app = builder.Build();

// Use Infra Middlewares
app
    .UseExceptionMiddleware()
    .UseRouting()
    .UseCorsPolicy()
    .UseCurrentUser()
    .UseBackgroundJobs(config, options =>
    {
        options.Dashboard = true;
        options.Scheduler = true;
    })
    .UseAuthorization()
    .UseOpenApiDocumentation(config);

app.MapControllers().RequireAuthorization();

app.MapGet("/", () => "ApiTemplate Users API")
    .ExcludeFromDescription();

app.Run();
