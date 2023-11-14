using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RaysGitOpsDemo.Chassis.Swagger;

/// <summary>
/// Configures the Swagger generation options.
/// </summary>
/// <remarks>This allows API versioning to define a Swagger document per API version after the
/// <see cref="IApiVersionDescriptionProvider"/> service has been resolved from the service container.</remarks>
internal class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
  private readonly IApiVersionDescriptionProvider _provider;
  private readonly SwaggerConfiguration _configuration;

  /// <summary>
  /// Instantiates a new ConfigureSwaggerOptions.
  /// </summary>
  public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IOptions<SwaggerConfiguration> configuration)
  {
    _provider = provider;
    _configuration = configuration.Value;
  }

  /// <inheritdoc />
  public void Configure(SwaggerGenOptions options)
  {
    // add a swagger document for each discovered API version
    // note: you might choose to skip or document deprecated API versions differently
    foreach (var description in _provider.ApiVersionDescriptions)
    {
      options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
    }
  }

  private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
  {
    var info = new OpenApiInfo
    {
      Title = _configuration.Title,
      Version = description.ApiVersion.ToString(),
      Description = _configuration.Description,
    };

    if (description.IsDeprecated)
    {
      info.Description += _configuration.DeprecatedVersionWarning;
    }

    return info;
  }
}