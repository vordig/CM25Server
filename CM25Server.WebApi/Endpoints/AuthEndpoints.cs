using CM25Server.Domain.Commands;
using CM25Server.Services;
using CM25Server.Services.Core;
using CM25Server.WebApi.ApiVersioning;

namespace CM25Server.WebApi.Endpoints;

public static class AuthEndpoints
{
    private const string LoginEndpointName = "Login";
    
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints,
        ApiVersionManager apiVersionManager)
    {
        var api = apiVersionManager.GetApi(endpoints, "auth", "Auth", ApiVersionManager.ApiVersions.V1_0);

        api.MapAuthEndpoints();

        return endpoints;
    }
    
    private static void MapAuthEndpoints(this IEndpointRouteBuilder api)
    {
        api
            .MapPost("login", LoginAsync)
            .WithName(LoginEndpointName)
            .Produces<AuthResponse>();
    }
    
    private static async Task<IResult> LoginAsync(LoginCommand command, AuthService authService,
        CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(command, cancellationToken);
        return result.Match(Results.Ok, Results.BadRequest);
    }
}