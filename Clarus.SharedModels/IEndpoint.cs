using Microsoft.AspNetCore.Routing;

namespace Clarus.SharedModels;

public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder app);
}
