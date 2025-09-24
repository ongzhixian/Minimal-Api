using Clarus.Endpoints;


using Microsoft.OpenApi.Models;

using Scalar.AspNetCore;

using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog();

    #region CONFIGURATIONS -- TO MOVE TO CONFIGURATIONS FOLDER AT A LATER DATE

    builder.Services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
    {
        options.AddServerHeader = false;
    });

    //builder.Services.AddCors(options =>
    //{
    //    const string MyAllowSpecificOrigins = "sdad";
    //    // Check if the application is running in a development environment.
    //    if (builder.Environment.IsDevelopment())
    //    {
    //        // For development, we can be more permissive to ease testing.
    //        // This policy allows any origin, method, and header.
    //        options.AddPolicy(name: MyAllowSpecificOrigins,
    //                          policy =>
    //                          {
    //                              policy.AllowAnyOrigin()
    //                                    .AllowAnyHeader()
    //                                    .AllowAnyMethod();
    //                          });
    //    }
    //    else // Production or other environments
    //    {
    //        // In production, we should only allow specific, trusted origins.
    //        // This makes your API more secure by preventing access from unwanted domains.
    //        // The allowed origins are read from the appsettings.json file.
    //        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
    //        if (allowedOrigins != null)
    //        {
    //            options.AddPolicy(name: MyAllowSpecificOrigins,
    //                              policy =>
    //                              {
    //                                  policy.WithOrigins(allowedOrigins)
    //                                        .AllowAnyHeader()
    //                                        .AllowAnyMethod();
    //                              });
    //        }
    //    }
    //});

    var swaggerDocsConfiguration = new Dictionary<string, OpenApiInfo>();
    builder.Configuration.GetSection("SwaggerDocs").Bind(swaggerDocsConfiguration);

    #endregion

    builder.Services.AddOpenApi(); // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        Console.WriteLine("Numer of config found: {0}", swaggerDocsConfiguration.Count);
        foreach (var config in swaggerDocsConfiguration)
            options.SwaggerDoc(config.Key, config.Value);
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var swaggerUiConfiguration = new SwaggerUiConfiguration();
            builder.Configuration.GetSection("SwaggerUi").Bind(swaggerUiConfiguration);

            options.DocumentTitle = swaggerUiConfiguration.DocumentTitle;
            // Route prefix to access Swagger UI; default is "/swagger/index.html"
            options.RoutePrefix = swaggerUiConfiguration.RoutePrefix;

            foreach (var config in swaggerDocsConfiguration)
                options.SwaggerEndpoint($"/{swaggerUiConfiguration.RoutePrefix}/{config.Key}/swagger.json", config.Key);

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

    //Minimal.WebApi.Routes.WeatherForecast.RegisterEndpoints(app);
    //app.RegisterWeatherForecastApiEndpoints();
    //app.RegisterWeatherForecastApiEndpoints();
    app.RegisterSampleApiEndpoints();
    //app.RegisterEndpoints(); // Modular registration
    //app.RegisterEndpoints();

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