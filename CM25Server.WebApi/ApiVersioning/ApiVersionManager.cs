using Asp.Versioning;
using Asp.Versioning.Builder;

namespace CM25Server.WebApi.ApiVersioning;

public class ApiVersionManager
{
    private const string BaseNeutralApiUrl = "api";
    private const string BaseVersionedApiUrl = "api/v{version:apiVersion}";

    private ApiVersionSet _apiVersionSet = default!;

    public ApiVersionManager WithApiVersionSet(ApiVersionSet apiVersionSet)
    {
        _apiVersionSet = apiVersionSet;
        return this;
    }

    public IEndpointRouteBuilder GetNeutralApi(IEndpointRouteBuilder endpoints, string urlPrefix, string groupName)
    {
        var routeGroup = endpoints
            .MapGroup($"{BaseNeutralApiUrl}/{urlPrefix}")
            .WithTags(groupName)
            .WithApiVersionSet(_apiVersionSet)
            .IsApiVersionNeutral();
        return routeGroup;
    }

    public IEndpointRouteBuilder GetApi(IEndpointRouteBuilder endpoints, string urlPrefix, string groupName,
        params ReadOnlySpan<ApiVersion> apiVersions)
    {
        var routeGroup = endpoints
            .MapGroup($"{BaseVersionedApiUrl}/{urlPrefix}")
            .WithTags(groupName)
            .WithApiVersionSet(_apiVersionSet);

        foreach (var apiVersion in apiVersions)
            routeGroup.MapToApiVersion(apiVersion);

        return routeGroup;
    }

    public static class ApiVersions
    {
        public static ApiVersion V1_0 = new(1.0);
    }
}