using System.Reflection;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using RaysGitOpsDemo.Chassis.Common;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RaysGitOpsDemo.Chassis.Swagger;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to add Swagger docs.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Add Swashbuckle and Swagger to dependency injection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to inject into.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> object created from appsettings.</param>
    /// <returns>The same <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="ArgumentNullException">If any parameter is null.</exception>
    /// <exception cref="InvalidOperationException">If configuration contains an unsupported ApiVersionReader.</exception>
    public static IServiceCollection AddChassisSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services = services ?? throw new ArgumentNullException(nameof(services));
        configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        services.Configure<SwaggerConfiguration>(configuration.GetSection($"{Constants.ConfigurationSection}:Swagger"));
        var provider = services.BuildServiceProvider();
        var swaggerConfig = provider.GetRequiredService<IOptions<SwaggerConfiguration>>().Value;

        services.AddApiVersioning(options =>
        {
            switch (swaggerConfig.ApiVersionReader)
            {
                case ApiVersionReaderTypes.QueryString:
                    options.ApiVersionReader = new QueryStringApiVersionReader(swaggerConfig.ApiVersionParameterName);
                    break;

                case ApiVersionReaderTypes.Header:
                    options.ApiVersionReader = new HeaderApiVersionReader(swaggerConfig.ApiVersionParameterName);
                    break;

                case ApiVersionReaderTypes.MediaType:
                    options.ApiVersionReader = new MediaTypeApiVersionReader(swaggerConfig.ApiVersionParameterName);
                    break;

                case ApiVersionReaderTypes.UrlSegment:
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                    break;

                default:
                    throw new InvalidOperationException($"Invalid ApiVersionReaders type: [{swaggerConfig.ApiVersionReader}].");
            }
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = swaggerConfig.ApiVersionFormat;
            options.SubstituteApiVersionInUrl = true;
        });

        if (swaggerConfig.Enabled)
        {
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddSwaggerGen(options =>
            {
                // Add a custom operation filter which sets default values
                options.OperationFilter<SwaggerDefaultValues>();

                if (swaggerConfig.EnableAnnotations)
                {
                    options.EnableAnnotations(true, true);
                }

                // Integrate xml comments
                options.IncludeXmlComments(swaggerConfig.XmlCommentFilePath ?? GuessXmlCommentFilePath);
            });
        }

        return services;
    }

    /// <summary>
    /// If no path to the XML doc file is specified explicitly, assume it is in the same
    /// directory as the assemblies themselves.
    /// </summary>
    private static string GuessXmlCommentFilePath
    {
        get
        {
            var xmlFile = $"{Assembly.GetEntryAssembly()?.GetName()?.Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            return xmlPath;
        }
    }
}
