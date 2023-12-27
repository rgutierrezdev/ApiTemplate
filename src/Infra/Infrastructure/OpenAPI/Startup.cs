using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ApiTemplate.Application.Common.Validators;
using ZymLabs.NSwag.FluentValidation;

namespace ApiTemplate.Infrastructure.OpenAPI;

public static class Startup
{
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services, IConfiguration config)
    {
        var settings = config.GetSection(nameof(SwaggerSettings)).Get<SwaggerSettings>();

        if (settings?.Enable == true)
        {
            services
                .AddEndpointsApiExplorer()
                .AddOpenApiDocument((options, serviceProvider) =>
                {
                    options.PostProcess = document => { document.Info.Title = settings.Title; };

                    // to describe model query params in camelCase
                    options.DocumentProcessors.Add(new CamelCaseParameterProcessor());

                    var fluentValidationSchemaProcessor = serviceProvider
                        .CreateScope()
                        .ServiceProvider
                        .GetService<FluentValidationSchemaProcessor>();

                    // Add the fluent validations schema processor
                    options.SchemaProcessors.Add(fluentValidationSchemaProcessor);

                    options.OperationProcessors.Add(new AllowAnonymousAttributeProcessor());
                    options.OperationProcessors.Add(new AuthPermissionAttributeProcessor());
                    options.OperationProcessors.Add(new OperationErrorsAttributeProcessor());

                    options.FlattenInheritanceHierarchy = true;
                })
                .Configure<JsonOptions>(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                    );
                });

            services.AddScoped<FluentValidationSchemaProcessor>(provider =>
            {
                var validationRules = provider.GetService<IEnumerable<FluentValidationRule>>()!.ToList();
                var loggerFactory = provider.GetService<ILoggerFactory>();

                // Show pattern when Equal validator is applied to attribute
                validationRules.Add(new FluentValidationRule("Equal")
                {
                    Matches = propertyValidator => propertyValidator is IEqualValidator,
                    Apply = context =>
                    {
                        var equalValidator = (IEqualValidator)context.PropertyValidator;
                        var pattern = equalValidator.ValueToCompare?.ToString();

                        if (pattern != null && equalValidator.ValueToCompare is bool)
                            pattern = pattern.ToLower();

                        var schema = context.SchemaProcessorContext.Schema;
                        schema.Properties[context.PropertyKey].Pattern = " " + pattern;
                    }
                });

                validationRules.Add(new FluentValidationRule("In")
                {
                    Matches = propertyValidator => propertyValidator is IInValidator,
                    Apply = context =>
                    {
                        var inValidator = (IInValidator)context.PropertyValidator;
                        var property = context.SchemaProcessorContext.Schema.Properties[context.PropertyKey];
                        var values = (string[])inValidator.ValueToCompare;

                        foreach (var value in values)
                        {
                            property.Enumeration.Add(value);
                        }
                    }
                });

                return new FluentValidationSchemaProcessor(provider, validationRules, loggerFactory);
            });
        }

        return services;
    }

    public static IApplicationBuilder UseOpenApiDocumentation(this IApplicationBuilder app, IConfiguration config)
    {
        var settings = config.GetSection(nameof(SwaggerSettings)).Get<SwaggerSettings>();

        if (settings?.Enable == true)
        {
            app.UseOpenApi();
            app.UseSwaggerUi3(options =>
            {
                options.DefaultModelsExpandDepth = -1;
                options.DocExpansion = "list";
                options.DocumentTitle = settings.Title;
            });
            app.UseReDoc(options => { options.Path = "/redoc"; });
        }

        return app;
    }
}
