using CM25Server.WebApi.ApiVersioning;
using CM25Server.WebApi.Endpoints;

namespace CM25Server.WebApi.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder UseEndpoints(this IEndpointRouteBuilder endpoints,
        ApiVersionManager apiVersionManager)
    {
        endpoints
            .MapProjectEndpoints(apiVersionManager)
            .MapIssueEndpoints(apiVersionManager);

        return endpoints;
    }
}