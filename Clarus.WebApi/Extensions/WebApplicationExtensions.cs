using ReadyPerfectly.AspNetCore;
using ReadyPerfectly.Utilities;

namespace Clarus.Extensions;

public static partial class WebApplicationExtensions
{
    public static void MapApiEndpoints(this WebApplication app)
    {
        var apiEndpointMappers = TypeUtility.GetInstancesOfTypesImplementing<IApiEndpointMapper>(app.Services);

        foreach (var apiEndpointMapper in apiEndpointMappers)
        {
            apiEndpointMapper.MapApiEndpoints(app);
        }
    }
}
