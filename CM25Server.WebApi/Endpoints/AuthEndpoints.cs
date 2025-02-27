using CM25Server.Domain.Commands;
using CM25Server.Services;
using CM25Server.Services.Core;
using CM25Server.WebApi.ApiVersioning;
using CM25Server.WebApi.Extensions;

namespace CM25Server.WebApi.Endpoints;

public static class AuthEndpoints
{
    private const string SignInEndpointName = "Sign In";
    private const string SignUpEndpointName = "Sign Up";
    private const string RefreshEndpointName = "Refresh";

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
            .MapPost("sign-in", SignInAsync)
            .WithName(SignInEndpointName)
            .Produces<AuthResponse>();
        
        api
            .MapPost("sign-up", SignUpAsync)
            .WithName(SignUpEndpointName)
            .Produces<AuthResponse>();
        
        api
            .MapPost("refresh", RefreshAsync)
            .WithName(RefreshEndpointName)
            .Produces<AuthResponse>();
    }

    private static async Task<IResult> SignInAsync(SignInCommand command, AuthService authService,
        CancellationToken cancellationToken)
    {
        var result = await authService.SignInAsync(command, cancellationToken);
        return result.ToOkResponse();
    }
    
    private static async Task<IResult> SignUpAsync(SignUpCommand command, AuthService authService,
        CancellationToken cancellationToken)
    {
        var result = await authService.SignUpAsync(command, cancellationToken);
        return result.ToOkResponse();
    }
    
    private static async Task<IResult> RefreshAsync(RefreshCommand command, AuthService authService,
        CancellationToken cancellationToken)
    {
        var result = await authService.RefreshAsync(command, cancellationToken);
        return result.ToOkResponse();
    }
}