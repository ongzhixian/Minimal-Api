using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using ReadyPerfectly.AspNetCore;
using ReadyPerfectly.OpenApi.Extensions;
using ReadyPerfectly.Swagger;

namespace Clarus.ApiDefinitions;

[ApiDefinition(DocumentId = "clarus-service-api", Title = "Clarus Service API",
    Description = "APIs for Clarus", Version = "v1")]
public class ClarusServiceApi : IApiEndpointMapper
{
    private readonly ILogger logger = null!;


    public ClarusServiceApi(ILogger<ClarusServiceApi> logger)
    {
        this.logger = logger;
    }

    public void MapApiEndpoints(IEndpointRouteBuilder endpointRouteBuilder)
    {
        MapHealthApiEndPoints(endpointRouteBuilder.MapGroup("health"));

        MapDateTimeApiEndPoints(endpointRouteBuilder.MapGroup("datetime"));

        //MapTimeZoneApiEndPoints(endpointRouteBuilder.MapGroup("timezone"));
    }

    private void MapHealthApiEndPoints(RouteGroupBuilder routeGroupBuilder)
    {
        routeGroupBuilder.MapGet(string.Empty, () =>
        {
            return Results.Ok(HealthStatus.Healthy.ToString());
        }).Produces<string>(StatusCodes.Status200OK)
        .WithOpenApi(operation =>
        {
            operation.Summary = "Gets API health";
            operation.Description = "Returns the health status of APIs";
            operation.OperationId = "api-health";
            operation.Tags = [new() { Name = "Health" }];

            return operation;
        });

    }

    private void MapDateTimeApiEndPoints(RouteGroupBuilder routeGroupBuilder)
    {
        var timeZoneInfoExamples = TimeZoneInfo.GetSystemTimeZones().ToDictionary(r => r.Id, r => new OpenApiExample
        {
            Summary = $"{r.BaseUtcOffset} {r.StandardName}",
            Value = new OpenApiString(r.Id)
        });

        var responseExamples = new Dictionary<string, OpenApiExample>
        {
            ["taipei"] = new OpenApiExample
            {
                Summary = "Taipei Standard Time",
                Value = new OpenApiString("2025-10-04T08:12:59+08:00")
            },
            ["singapore"] = new OpenApiExample
            {
                Summary = "Singapore Standard Time",
                Value = new OpenApiString("2025-10-04T08:12:59+09:00")
            }
        };

        routeGroupBuilder.MapGet(string.Empty, GetDateTimeForTimeZone)
        //.Produces<DateTime>(StatusCodes.Status200OK)        // uses builder.WithMetadata(new ProducesResponseTypeMetadata(statusCode, responseType ?? typeof(void), contentTypes));
        .WithName(nameof(GetDateTimeForTimeZone))           // uses builder.WithMetadata(new EndpointNameMetadata(endpointName), new RouteNameMetadata(endpointName));
        .WithSummary("Gets date time")                      // uses builder.WithMetadata(new EndpointSummaryAttribute(summary));
        .WithDescription("Returns the datetime of server")  // uses builder.WithMetadata(new EndpointDescriptionAttribute(description));
        .WithTags("DateTime")                               // uses builder.WithMetadata(new TagsAttribute(tags));
        //.WithOpenApi()                                      // uses builder.Finally(builder => AddAndConfigureOperationForEndpoint(builder));
        //.WithOpenApi(operation =>
        //{
        //    // Note: Use WithOpenApi() extension with caution;
        //    // I think the generated OpenApi JSON differs between built-in and Swagger.
        //    //operation.Summary = "Gets date time";
        //    //operation.Description = "Returns the datetime of server";
        //    ////operation.OperationId = nameof(GetDateTimeForTimeZone);
        //    //operation.Tags = [new() { Name = "DateTime" }];

        //    // TODO: Simplify this:
        //    if (operation.Parameters
        //        .FirstOrDefault(r => r.Name.Equals("timeZoneInfoId", StringComparison.InvariantCultureIgnoreCase))
        //        is OpenApiParameter parameter)
        //    {
        //        // For server
        //        parameter.Schema = new OpenApiSchema
        //        {
        //            Type = "string",
        //            //Format = "string", 
        //            //Default = new OpenApiString("asd"),
        //            //Example = new OpenApiString("UTC")
        //        };

        //        parameter.Examples = TimeZoneInfo.GetSystemTimeZones().ToDictionary(r => r.Id, r => new OpenApiExample
        //        {
        //            Summary = $"{r.BaseUtcOffset} {r.StandardName}",
        //            Value = new OpenApiString(r.Id)
        //        });

        //        parameter.Example = parameter.Examples["Singapore Standard Time"].Value;
        //    }


        //    var responseExamples = new Dictionary<string, OpenApiExample>
        //    {
        //        ["taipei"] = new OpenApiExample
        //        {
        //            Summary = "Taipei Standard Time",
        //            Value = new OpenApiString("2025-10-04T08:12:59+08:00")
        //        },
        //        ["singapore"] = new OpenApiExample
        //        {
        //            Summary = "Singapore Standard Time",
        //            Value = new OpenApiString("2025-10-04T08:12:59+09:00")
        //        }
        //    };

        //    operation.Responses["200"].Content = new Dictionary<string, OpenApiMediaType>
        //    {
        //        ["application/json"] = new OpenApiMediaType
        //        {
        //            Schema = new OpenApiSchema
        //            {
        //                Type = "string",
        //                Format = "date-time"
        //            },
        //            Example = new OpenApiString("2025-10-04T08:12:59+09:00"), // Singapore as default
        //            Examples = responseExamples,
        //        }
        //    };

        //    return operation;
        //})
        .WithOperationParameter("timeZoneInfoId", new OpenApiSchema
        {
            Type = "string",
        }, timeZoneInfoExamples, timeZoneInfoExamples["Singapore Standard Time"].Value) // Hmm...kind of awkward looking; shelved refactoring for now
        .WithOperationResponse("200", new Dictionary<string, OpenApiMediaType>
        {
            ["application/json"] = new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Format = "date-time"
                },
                Examples = responseExamples,
                Example = responseExamples["singapore"].Value,
                
                //Example = new OpenApiString("2025-10-04T08:12:59+09:00"), // Singapore as default
            }
        })
        .WithOperation()
        
        //.DebugOpenApiDocumentation()
        ;
    }


    private Results<BadRequest, Ok<DateTime>> GetDateTimeForTimeZone([Microsoft.AspNetCore.Mvc.FromQuery]string timeZoneInfoId)
    //private IResult GetDateTimeForTimeZone([Microsoft.AspNetCore.Mvc.FromQuery] string timeZoneInfoId)
    {
        var timeZoneInfo = TimeZoneInfo.GetSystemTimeZones()
            .FirstOrDefault(r => r.Id.Equals(timeZoneInfoId, StringComparison.InvariantCultureIgnoreCase))
            ?? TimeZoneInfo.Utc;

        System.Diagnostics.Debugger.Break();

        var result = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);
        logger.LogInformation("{FunctionName} return {GetDateTimeForTimeZoneResult}", nameof(GetDateTimeForTimeZone), result);

        // Why 
        //IResult x1 = Results.Ok<DateTime>(result);
        //Microsoft.AspNetCore.Http.HttpResults.Ok<DateTime> x2 = TypedResults.Ok<DateTime>(result);

        return TypedResults.Ok<DateTime>(result);   // uses return union type Results<BadRequest, Ok<DateTime>> // 
        //return Results.Ok<DateTime>(result);      // uses return type IResult

        // Why use TypedResults over Results
        // Functionally, they don't make much of a difference.
        // The main area where they affect are in unit testing and OpenApi
        // Using IResult as return type means we need to cast it unit testing.
        // In the context of OpenApi, it means we NEED to specify a Produces*Attribute
        // We can omit Produces*Attribute using TypedResults

    }

    private void MapTimeZoneApiEndPoints(RouteGroupBuilder routeGroupBuilder)
    {
        routeGroupBuilder.MapGet(string.Empty, () =>
        {
            return Results.Ok(TimeZoneInfo.GetSystemTimeZones());
        }).Produces<string>(StatusCodes.Status200OK)
        //.WithOpenApi(operation =>
        //{
        //    operation.Summary = "Gets API health";
        //    operation.Description = "Returns the health status of APIs";
        //    operation.OperationId = "api-health";
        //    operation.Tags = [new() { Name = "Health" }];

        //    return operation;
        //})
        ;

    }
}
