using System.Net.Mime;

using Clarus.Extensions;

using Microsoft.OpenApi.Models;

namespace Clarus.Endpoints;

public static partial class WebApplicationExtensions
{
    public static void RegisterSampleApiEndpoints(this WebApplication app)
    {
        const string routePrefix = "sample";
        //const string routePrefix = nameof(EchoApi).ToLower(); // or just "echo" if you prefer static

        RouteGroupBuilder apiGroup = app.MapGroup(routePrefix);

        SampleApi api = app.Services.GetRequiredService<SampleApi>();
        
        apiGroup.MapGet("echo", api.GetEcho)
            //.Accepts<string>(System.Net.Mime.MediaTypeNames.Text.Plain)
            .Produces<string>(StatusCodes.Status200OK)
            //.Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(api.GetEchoOpenApiOperation);

        apiGroup.MapGet("datetime", api.GetDateTime)
            .Produces<string>(StatusCodes.Status200OK)
            .WithOpenApi(api.GetDateTimeOpenApiOperation);

    }
}

public class SampleApi
{
    private readonly ILogger logger;

    public void RegisterEndpoints(WebApplication app)
    {

    }

    public SampleApi(ILogger<SampleApi> logger)
    {
        this.logger = logger;
    }

    public IResult GetEcho(HttpContext httpContext, string message = "hello")
    {
        logger.LogInformation("Return message");

        return Results.Text(message);
    }

    public OpenApiOperation GetEchoOpenApiOperation(OpenApiOperation op)
    {
        op.Summary = "Echoes message";
        op.Description = "Returns the echoed message from query parameter";
        op.OperationId = nameof(GetEcho);
        op.Tags = [new() { Name = "Echo" }];
        op.Parameters = new List<OpenApiParameter>
        {
            new()
            {
                Name = "message",
                In = ParameterLocation.Query,
                Required = false,
                Description = "Message to echo",
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Default = new Microsoft.OpenApi.Any.OpenApiString("hello")
                }
            }
    };

        op.Responses = new OpenApiResponses
        {
            ["200"] = new()
            {
                Description = "Successful response with echoed message",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    [MediaTypeNames.Text.Plain] = new()
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Example = new Microsoft.OpenApi.Any.OpenApiString("hello")
                        }
                    }
                }
            }
            //,
            //["404"] = new()
            //{
            //    Description = "The requested resource could not be found.",
            //    Content = new Dictionary<string, OpenApiMediaType>
            //    {
            //        [MediaTypeNames.Application.Json] = CreateErrorMediaType(),
            //        [MediaTypeNames.Application.Xml] = CreateErrorMediaType()
            //    }
            //}
        };

        return op;
    }

    // 
    public IResult GetDateTime(HttpContext httpContext)
    {
        logger.LogInformation("Return message");

        return Results.Ok<DateTime>(DateTime.Now);
    }


    public OpenApiOperation GetDateTimeOpenApiOperation(OpenApiOperation op)
    {
        op.Summary = "Gets date time";
        op.Description = "Returns the datetime of server";
        op.OperationId = nameof(GetDateTime);
        op.Tags = [new() { Name = "DateTime" }];
        //op.Parameters = new List<OpenApiParameter>
        //{
        //    new()
        //    {
        //        Name = "message",
        //        In = ParameterLocation.Query,
        //        Required = false,
        //        Description = "Message to echo",
        //        Schema = new OpenApiSchema
        //        {
        //            Type = "string",
        //            Default = new Microsoft.OpenApi.Any.OpenApiString("hello")
        //        }
        //    }
        //};

        //op.Responses = new OpenApiResponses
        //{
        //    ["200"] = new()
        //    {
        //        Description = "Successful response with echoed message",
        //        Content = new Dictionary<string, OpenApiMediaType>
        //        {
        //            [MediaTypeNames.Text.Plain] = new()
        //            {
        //                Schema = new OpenApiSchema
        //                {
        //                    Type = "string",
        //                    Example = new Microsoft.OpenApi.Any.OpenApiString("hello")
        //                }
        //            }
        //        }
        //    }
        //    //,
        //    //["404"] = new()
        //    //{
        //    //    Description = "The requested resource could not be found.",
        //    //    Content = new Dictionary<string, OpenApiMediaType>
        //    //    {
        //    //        [MediaTypeNames.Application.Json] = CreateErrorMediaType(),
        //    //        [MediaTypeNames.Application.Xml] = CreateErrorMediaType()
        //    //    }
        //    //}
        //};

        return op;
    }


    //private static OpenApiTag CreateTag(string name) => new()
    //{
    //    Name = name,
    //    Description = $"{name} operations",
    //    ExternalDocs = new()
    //    {
    //        Description = "Echo API documentation",
    //        Url = new("/docs/echo", UriKind.Relative)
    //    }
    //};

    //private static OpenApiMediaType CreateErrorMediaType() => new()
    //{
    //    Schema = new()
    //    {
    //        Type = "string",
    //        Example = new Microsoft.OpenApi.Any.OpenApiString("Resource not found.")
    //    }
    //};

}


public record EchoRequest(string Message);

public class EchoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/echo", (HttpContext context, EchoRequest request) =>
        {
            if (!context.Request.ContentType?.Contains("application/json") ?? true)
            {
                return Results.StatusCode(StatusCodes.Status415UnsupportedMediaType);
            }

            return Results.Text($"Echo: {request.Message}");
        })
        .WithName("EchoMessage")
        .WithOpenApi(op =>
        {
            op.Summary = "Echo a message";
            op.Description = "Returns the echoed message from the request body";
            op.Tags = [new() { Name = "Echo" }];

            op.RequestBody = new OpenApiRequestBody
            {
                Description = "Echo request",
                Required = true,
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new()
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["message"] = new() { Type = "string" }
                            }
                        }
                    }
                }
            };

            op.Responses["200"] = new()
            {
                Description = "Echoed message",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["text/plain"] = new()
                    {
                        Schema = new OpenApiSchema { Type = "string" }
                    }
                }
            };

            return op;
        });
    }
}
