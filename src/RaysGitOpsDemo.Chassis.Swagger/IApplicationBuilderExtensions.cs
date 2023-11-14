using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace RaysGitOpsDemo.Chassis.Swagger;

/// <summary>
/// Extension methods for <see cref="IApplicationBuilder"/> to add Swagger to the
/// application pipeline.
/// </summary>
public static class IApplicationBuilderExtensions
{
    /// <summary>
    /// The route prefix for the Swagger endpoint.
    /// </summary>
    public const string SwaggerEndPoint = "/swagger";

    /// <summary>
    /// Add Swagger and SwaggerUI to the application pipeline.
    /// </summary>
    /// <param name="app"></param>
    public static IApplicationBuilder UseChassisSwagger(this IApplicationBuilder app)
    {
        app = app ?? throw new ArgumentNullException(nameof(app));
        var config = app.ApplicationServices.GetService<IOptions<SwaggerConfiguration>>()?.Value ?? new SwaggerConfiguration();
        var apiDescProvider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
        
        if (config.Enabled)
        {
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                foreach (var description in apiDescProvider.ApiVersionDescriptions.OrderByDescending(v => v.ApiVersion))
                {
                    options.SwaggerEndpoint($"{SwaggerEndPoint}/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });
        }

        return app;
    }
}