using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using RaysGitOpsDemo.Chassis.Swagger;

namespace RaysGitOpsDemo.Chassis;

/// <summary>
/// Extension methods for <see cref="IApplicationBuilder"/> to use the chassis.
/// </summary>
public static class IApplicationBuilderExtensions
{
    /// <summary>
    /// Use the chassis.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> to use the chassis.</param>
    /// <returns>The same <see cref="IApplicationBuilder"/>.</returns>
    /// <exception cref="ArgumentNullException">If any parameter is null.</exception>
    public static IApplicationBuilder UseChassis(this IApplicationBuilder builder)
    {
        builder = builder ?? throw new ArgumentNullException(nameof(builder));

        builder.UseChassisSwagger();

        return builder;
    }
}
