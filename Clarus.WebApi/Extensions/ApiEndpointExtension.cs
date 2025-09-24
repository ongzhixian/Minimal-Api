namespace Clarus.Extensions;

public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder app);
}

//public static class EndpointExtensions
//{
//    public static void RegisterEndpoints(this IEndpointRouteBuilder app)
//    {
//        EchoEndpoint.Map(app);
//        //EchoApiDemo.Endpoints.EchoEndpoint.Map(app);
//        // Add more endpoints here
//    }
//}
