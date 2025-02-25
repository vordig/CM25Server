using CM25Server.Domain.Commands;
using CM25Server.Infrastructure.Enums;
using CM25Server.Services;
using CM25Server.Services.Contracts;
using CM25Server.Services.Core;
using CM25Server.WebApi.ApiVersioning;
using CM25Server.WebApi.BindableData;
using CM25Server.WebApi.Extensions;

namespace CM25Server.WebApi.Endpoints;

public static class IssueEndpoints
{
    private const string AnyIssueEndpointName = "AnyIssue";
    private const string GetIssuesEndpointName = "GetIssues";
    private const string GetIssueEndpointName = "GetIssue";
    private const string CreateIssueEndpointName = "CreateIssue";
    private const string UpdateIssueEndpointName = "UpdateIssue";
    private const string DeleteIssueEndpointName = "DeleteIssue";

    public static IEndpointRouteBuilder MapIssueEndpoints(this IEndpointRouteBuilder endpoints,
        ApiVersionManager apiVersionManager)
    {
        var api = apiVersionManager.GetApi(endpoints, "issues", "Issues", ApiVersionManager.ApiVersions.V1_0);

        api.MapIssueEndpoints();

        return endpoints;
    }

    private static void MapIssueEndpoints(this IEndpointRouteBuilder api)
    {
        api
            .MapGet("{projectId:guid}/any", AnyIssueAsync)
            .WithName(AnyIssueEndpointName)
            .RequireAuthorization()
            .Produces<bool>();

        api
            .MapGet("{projectId:guid}", GetIssuesAsync)
            .WithName(GetIssuesEndpointName)
            .RequireAuthorization()
            .Produces<PageResponse<IssueListResponse>>();

        api
            .MapGet("{projectId:guid}/{id:guid}", GetIssueAsync)
            .WithName(GetIssueEndpointName)
            .RequireAuthorization()
            .Produces<IssueListResponse>();

        api
            .MapPost("{projectId:guid}", CreateIssueAsync)
            .WithName(CreateIssueEndpointName)
            .RequireAuthorization()
            .Produces<IssueListResponse>();

        api
            .MapPut("{projectId:guid}/{id:guid}", UpdateIssueAsync)
            .WithName(UpdateIssueEndpointName)
            .RequireAuthorization()
            .Produces<Guid>();

        api
            .MapDelete("{projectId:guid}/{id:guid}", DeleteIssueAsync)
            .WithName(DeleteIssueEndpointName)
            .RequireAuthorization()
            .Produces<Guid>();
    }

    private static async Task<IResult> AnyIssueAsync(Guid projectId, BindableAuthData bindableAuthData,
        IssueService issueService, CancellationToken cancellationToken)
    {
        var result = await issueService.AnyIssueAsync(projectId, bindableAuthData.UserId, cancellationToken);
        return result.ToOkResponse();
    }

    private static async Task<IResult> GetIssuesAsync(Guid projectId, BindableIssueFilteringData filteringData,
        BindableSortingData<IssueSortBy> sortingData, BindablePagingData pagingData,
        BindableAuthData bindableAuthData, IssueService issueService, CancellationToken cancellationToken)
    {
        var result = await issueService.GetIssuesAsync(filteringData, sortingData, pagingData,
            projectId, bindableAuthData.UserId, cancellationToken);
        return result.ToOkResponse();
    }

    private static async Task<IResult> GetIssueAsync(Guid id, Guid projectId, BindableAuthData bindableAuthData,
        IssueService issueService, CancellationToken cancellationToken)
    {
        var result = await issueService.GetIssueAsync(id, projectId, bindableAuthData.UserId, cancellationToken);
        return result.ToOkResponse();
    }

    private static async Task<IResult> CreateIssueAsync(Guid projectId, CreateIssueCommand command,
        BindableAuthData bindableAuthData, IssueService issueService, CancellationToken cancellationToken)
    {
        var result =
            await issueService.CreateIssueAsync(command, projectId, bindableAuthData.UserId, cancellationToken);
        return result.ToOkResponse();
    }

    private static async Task<IResult> UpdateIssueAsync(Guid id, Guid projectId, UpdateIssueCommand command,
        BindableAuthData bindableAuthData, IssueService issueService, CancellationToken cancellationToken)
    {
        var result =
            await issueService.UpdateIssueAsync(id, command, projectId, bindableAuthData.UserId, cancellationToken);
        return result.ToOkResponse();
    }

    private static async Task<IResult> DeleteIssueAsync(Guid id, Guid projectId, BindableAuthData bindableAuthData,
        IssueService issueService, CancellationToken cancellationToken)
    {
        var result = await issueService.DeleteIssueAsync(id, projectId, bindableAuthData.UserId, cancellationToken);
        return result.ToOkResponse();
    }
}