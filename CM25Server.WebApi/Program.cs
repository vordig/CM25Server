using System.Text;
using System.Text.Json.Serialization;
using CM25Server.Infrastructure.Core;
using CM25Server.Infrastructure.Core.Options;
using CM25Server.Infrastructure.Repositories;
using CM25Server.Services;
using CM25Server.WebApi.ApiVersioning;
using CM25Server.WebApi.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    RunApplication(args);

    Log.Information("Stopped cleanly");
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

static void RunApplication(string[] args)
{
    var builder = WebApplication.CreateSlimBuilder(args);

    builder.Host.UseSerilog();

    builder.Logging.ClearProviders();
    var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
    builder.Logging.AddSerilog(logger);
    Log.Logger = logger;

    builder.AddDefaultApiVersioning();

    builder.Services.AddOptionsWithValidateOnStart<AuthOptions>()
        .Bind(builder.Configuration.GetSection(AuthOptions.SectionName));

    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

    builder.Services.AddSingleton<IMongoDatabase>(_ =>
    {
        var database = DatabaseConfiguration.GetDatabase(Environment.GetEnvironmentVariable("CM25SERVER_DATABASE"));
        return database;
    });

    builder.Services.AddScoped<DatabaseContext>();

    builder.Services.AddScoped<ProjectRepository>();

    builder.Services.AddScoped<AuthService>();
    builder.Services.AddScoped<ProjectService>();
    builder.Services.AddScoped<IssueService>();

    var authOptions = builder.Configuration.GetSection(AuthOptions.SectionName).Get<AuthOptions>();
    if (authOptions is null) throw new ApplicationException("Auth options are missing");

    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // enable for https
        options.SaveToken = false;

        var key = Encoding.ASCII.GetBytes(authOptions.JWTSecret ?? throw new InvalidOperationException());

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = authOptions.JWTIssuer,
            ValidateAudience = true,
            ValidAudience = authOptions.JWTAudience,
            ClockSkew = TimeSpan.Zero
        };
    });
    builder.Services.AddAuthorization();

    DatabaseConfiguration.Configure();

    var app = builder.Build();

    app.UseDefaultApiVersioning();

    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            var apiVersion = httpContext.GetRequestedApiVersion();
            diagnosticContext.Set("ApiVersion", apiVersion?.ToString("'v'VVVV") ?? "Neutral");
        };
        options.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} version {ApiVersion} responded {StatusCode} in {Elapsed:0.0000} ms";
    });

    app.UseAuthentication();
    app.UseAuthorization();

    var apiVersionManager = app.Services.GetRequiredService<ApiVersionManager>();
    app.UseEndpoints(apiVersionManager);

    app.Run();
}