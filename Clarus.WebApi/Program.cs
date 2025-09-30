using Clarus.ApiDefinitions;
using Clarus.Extensions;

using Microsoft.OpenApi.Models;

using ReadyPerfectly.Swagger;

using Scalar.AspNetCore;

using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(builder.Configuration));

    builder.Services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
    {
        options.AddServerHeader = false;
    });

    // TODO: Add CORs


    // TODO: Keeping comments here until we refactor this! *fumes*
    // Define OpenApi definitions.
    // 2 approaches:
    // 1.   Define the definitions in appsettings.json
    // 2.   Tag API definitions with attributes, then discover them (using reflection)
    // Picking approach number 2 here:

    // Approach 2:
    // Get a default OpenApiInfo based on appsettings.json ('DefaultOpenApiInfo')
    var defaultOpenApiInfo = builder.GetDefaultOpenApiInfo();

    // Then get a list of API definitions using decorator based definitions
    // This has to be out here because we need to reuse these types to register SwaggerEndpoints for SwaggerUI
    var apiDefinitions = builder.GetApiDefinitions();

    // Approach 1:
    //var swaggerDocsConfiguration = new Dictionary<string, OpenApiInfo>();
    //builder.Configuration.GetSection("SwaggerDocs").Bind(swaggerDocsConfiguration);


    // TODO: Refactor to ReadyPerfectly.OpenApi
    builder.Services.AddOpenApi(); // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    foreach (var config in apiDefinitions)
    {
        builder.Services.AddOpenApi(config.DocumentId, (openApiOptions) =>
        {
            openApiOptions.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new OpenApiInfo(defaultOpenApiInfo)
                {
                    Version = config.Version,
                    Title = config.Title,
                    Description = config.Description,
                };
                return Task.CompletedTask;
            });
        });
    }

    // TODO: Refactor to ReadyPerfectly.Swagger
    builder.Services.AddSwaggerGen(options =>
    {
        foreach (var config in apiDefinitions)
        {
            options.SwaggerDoc(config.DocumentId, new OpenApiInfo(defaultOpenApiInfo)
            {
                Version = config.Version,
                Title = config.Title,
                Description = config.Description,
            });
        }

        // A possible way of using approach 1 here as well (combining both approach) ;-)
        //Console.WriteLine("Numer of config found: {0}", swaggerDocsConfiguration.Count);
        //foreach (var config in swaggerDocsConfiguration)
        //    options.SwaggerDoc(config.Key, config.Value);
    });

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddScoped<ClarusServiceApi>();

    builder.Services.AddHealthChecks();

    var app = builder.Build();

    // Configure the HTTP request pipeline.

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            // TODO: replace with following pattern
            // var defaultOpenApiInfo = builder.Configuration.GetOpenApiInfoFromSection("DefaultOpenApiInfo");
            var swaggerUiConfiguration = new SwaggerUiConfiguration();
            builder.Configuration.GetSection("SwaggerUi").Bind(swaggerUiConfiguration);

            options.DocumentTitle = swaggerUiConfiguration.DocumentTitle;
            // Route prefix to access Swagger UI; default is "/swagger/index.html"
            options.RoutePrefix = swaggerUiConfiguration.RoutePrefix;

            //foreach (var config in swaggerDocsConfiguration)
            //    options.SwaggerEndpoint($"/{swaggerUiConfiguration.RoutePrefix}/{config.Key}/swagger.json", config.Key);

            foreach (var config in apiDefinitions)
            {
                options.SwaggerEndpoint($"/{swaggerUiConfiguration.RoutePrefix}/{config.DocumentId}/swagger.json", config.DocumentId);
            }

            // Obsolete ; to remove after we get the definition corrected
            //options.SwaggerEndpoint("/swagger/01-EmptyApi/swagger.json", "01-EmptyApi");
            //options.SwaggerEndpoint("/swagger/SampleApi/swagger.json", "SampleApi");
            //options.SwaggerEndpoint("/swagger/MinimalExampleApi/swagger.json", "MinimalExampleApi");
            //options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        });

        #region SETUP FOR REDOC
        app.UseReDoc(options =>
        {
            options.DocumentTitle = "Open API - ReDoc";
            options.RoutePrefix = "api-docs"; // change relative path to the UI
            options.SpecUrl("/openapi/v1.json");
        });
        #endregion

        #region SETUP FOR SCALAR
        app.MapScalarApiReference();
        #endregion

        app.MapOpenApi();
    }

    app.UseHttpsRedirection();

    //app.UseCors(MyAllowSpecificOrigins);
    //app.UseAuthentication();
    //app.UseAuthorization();

    app.UseDefaultFiles();
    app.UseStaticFiles();

    // Registering endpoints

    app.MapApiEndpoints();

    app.MapHealthChecks("health");

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}