using Microsoft.AspNetCore.Mvc;

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

        //MapCurltureApiEndPoints(endpointRouteBuilder.MapGroup("culture"));
    }

    private void MapHealthApiEndPoints(RouteGroupBuilder routeGroupBuilder)
    {
        routeGroupBuilder.MapGet(string.Empty, () =>
        {
            return Results.Ok("Healthy - OK");
        }).Produces<DateTime>(StatusCodes.Status200OK)
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
        routeGroupBuilder.MapGet(string.Empty, GetDateTimeForTimeZone)
        //.Produces<DateTime>(StatusCodes.Status200OK)
        .WithOpenApi(operation =>
        {
            // Note: Use WithOpenApi() extension with caution;
            // I think the generated OpenApi JSON differs between built-in and Swagger.
            operation.Summary = "Gets date time";
            operation.Description = "Returns the datetime of server";
            operation.OperationId = nameof(GetDateTimeForTimeZone);
            operation.Tags = [new() { Name = "DateTime" }];
            return operation;
        });
        //.WithOpenApiOperation("My-OP-ID");


        routeGroupBuilder.MapGet("timezone-id", (string timeZoneInfoId = "UTC") =>
        {
            return TimeZoneInfo.GetSystemTimeZones().Select(r => r.Id).OrderBy(r => r);
        }).Produces<IEnumerable<string>>(StatusCodes.Status200OK)
        //.OpenApiDocumentation(operationId: "timezone-id")
        ;


        routeGroupBuilder.MapGet("timezone/{timeZoneInfoId}", ([FromRoute] string timeZoneInfoId) =>
        {
            if (TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneInfoId, out var systemTimeZone))
                return Results.Ok(systemTimeZone);

            return Results.NotFound($"{timeZoneInfoId}");
        })
            //.WithOpenApi(op =>
            //{
            //    op.Summary = "SOme this";
            //    return op;
            //})
            //.WithSummary("Some sumamry")
            //.WithDescription("Some description")
            //.WithTags("Tag 1", "Tag 2")
        .OpenApiDocumentation();

        //.Produces<TimeZoneInfo>(StatusCodes.Status200OK)
        //.Produces(StatusCodes.Status404NotFound)
        //.WithOpenApi(operation =>
        //{
        //    operation.OperationId = "some-operation-id";
        //    operation.Summary = "Get TimeZoneInfo";
        //    operation.Description = "Returns TimeZoneInfo of timeZoneInfoId";
        //    operation.Tags = [new() { Name = "DateTime" }];

        //    System.Diagnostics.Debugger.Break();
        //    //operation.Parameters
        //    //operation.Responses
        //    operation.Parameters[0].Description = "AOAMA";
        //    operation.Responses.Add("204", new OpenApiResponse
        //    {
        //        Description = "Some 204 desc",
        //    });

        //    return operation;
        //})
        ;
    }

    private void MapCurltureApiEndPoints(RouteGroupBuilder routeGroupBuilder)
    {
        routeGroupBuilder.MapGet("", (string timeZoneInfoId = "UTC") =>
        {
            var timeZoneInfo = TimeZoneInfo.GetSystemTimeZones()
                .FirstOrDefault(r => r.Id.Equals(timeZoneInfoId, StringComparison.InvariantCultureIgnoreCase))
                ?? TimeZoneInfo.Utc;

            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);
        }).Produces<DateTime>(StatusCodes.Status200OK);
    }



    // 
    public IResult GetDateTime(HttpContext httpContext)
    {
        logger.LogInformation("Return message");

        return Results.Ok(DateTime.Now);
    }

    private IResult GetDateTimeForTimeZone(string timeZoneInfoId = "UTC")
    {
        var timeZoneInfo = TimeZoneInfo.GetSystemTimeZones()
            .FirstOrDefault(r => r.Id.Equals(timeZoneInfoId, StringComparison.InvariantCultureIgnoreCase))
            ?? TimeZoneInfo.Utc;


        var result = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);
        logger.LogInformation("{FunctionName} return {GetDateTimeForTimeZoneResult}", nameof(GetDateTimeForTimeZone), result);

        return Results.Ok<DateTime>(result);
    }
}
