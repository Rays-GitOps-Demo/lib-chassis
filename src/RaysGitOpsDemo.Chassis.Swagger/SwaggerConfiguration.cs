namespace RaysGitOpsDemo.Chassis.Swagger;

/// <summary>
/// This class encapsulates configurations for the Swagger/Swashbuckle portions of the chassis.
/// </summary>
internal class SwaggerConfiguration
{
    /// <summary>
    /// If true, publish Swagger documentation at the /swagger path.  Defaults to false.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// The name of the API to be displayed in SwaggerUI.
    /// </summary>
    public string Title { get; set; } = "Hello, world!";

    /// <summary>
    /// The description of the API to be displayed in SwaggerUI.
    /// </summary>
    public string Description { get; set; } = "It's, like, an API!";

    /// <summary>
    /// The text to be used in Swagger UI to indicate deprecated API versions.
    /// </summary>
    public string DeprecatedVersionWarning { get; set; } = " This API version has been deprecated.";

    /// <summary>
    /// Indicates where in the request the API shall look for the version being invoked. Defaults to
    /// <see cref="ApiVersionReaderTypes.QueryString" />.
    /// </summary>
    public ApiVersionReaderTypes ApiVersionReader { get; set; } = ApiVersionReaderTypes.QueryString;

    /// <summary>
    /// The name of the parameter that indicates the api Version being invoked. Defaults to "api-version".
    /// </summary>
    public string ApiVersionParameterName { get; set; } = "api-version";

    /// <summary>
    /// The expected format of the API version. Defaults to "v{major}[.{minor}][-{status}]"
    /// </summary>
    /// <remarks>
    /// https://github.com/microsoft/aspnet-api-versioning/wiki/Version-Format
    /// </remarks>
    public string ApiVersionFormat { get; set; } = "'v'VVV";

    /// <summary>
    /// The path to the XML file containing the documentation to be displayed in SwaggerUI. This
    /// should only be set if the file is not in the same directory as the assemblies themselves.
    /// </summary>
    public string XmlCommentFilePath { get; set; } = "";

    /// <summary>
    /// If set to true swagger annotations will be enabled and be displayed in SwaggerUI.
    /// </summary>
    public bool EnableAnnotations { get; set; }
}

internal enum ApiVersionReaderTypes
{
    QueryString = 0,
    Header,
    MediaType,
    UrlSegment
}