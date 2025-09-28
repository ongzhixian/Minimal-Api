using Microsoft.OpenApi.Models;

using ReadyPerfectly.OpenApi.Extensions;
using ReadyPerfectly.Swagger;
using ReadyPerfectly.Utilities;

namespace Clarus.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static OpenApiInfo GetDefaultOpenApiInfo(
        this WebApplicationBuilder builder, string configurationSectionName = "DefaultOpenApiInfo")
    {
        return builder.Configuration.GetOpenApiInfoFromSection(configurationSectionName);
    }

    public static IEnumerable<ApiDefinitionAttribute> GetApiDefinitions(
        this WebApplicationBuilder builder)
    {
        return TypeUtility.GetTypesWithAttribute<ApiDefinitionAttribute>();
    }
    
}
