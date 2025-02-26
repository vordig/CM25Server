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
    private const string CountIssuesEndpointName = "CountIssues";
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
            .MapGet("count", CountAsync)
            .WithName(CountIssuesEndpointName)
            .RequireAuthorization()
            .Produces<long>();

        api
            .MapGet("", GetIssuesAsync)
            .WithName(GetIssuesEndpointName)
            .RequireAuthorization()
            .Produces<PageResponse<IssueListResponse>>();

        api
            .MapGet("{id:guid}", GetIssueAsync)
            .WithName(GetIssueEndpointName)
            .RequireAuthorization()
            .Produces<IssueListResponse>();

        api
            .MapPost("", CreateIssueAsync)
            .WithName(CreateIssueEndpointName)
            .RequireAuthorization()
            .Produces<IssueListResponse>();

        api
            .MapPut("{id:guid}", UpdateIssueAsync)
            .WithName(UpdateIssueEndpointName)
            .RequireAuthorization()
            .Produces<Guid>();

        api
            .MapDelete("{id:guid}", DeleteIssueAsync)
            .WithName(DeleteIssueEndpointName)
            .RequireAuthorization()
            .Produces<Guid>();
    }

    private static async Task<IResult> CountAsync(BindableIssueFilteringData filteringData,
        BindableAuthData bindableAuthData, IssueService issueService, CancellationToken cancellationToken)
    {
        var result = await issueService.CountIssuesAsync(filteringData, bindableAuthData.UserId, cancellationToken);
        return result.ToOkResponse();
    }

    private static async Task<IResult> GetIssuesAsync(BindableIssueFilteringData filteringData,
        BindableSortingData<IssueSortBy> sortingData, BindablePagingData pagingData,
        BindableAuthData bindableAuthData, IssueService issueService, CancellationToken cancellationToken)
    {
        var result = await issueService.GetIssuesAsync(filteringData, sortingData, pagingData, bindableAuthData.UserId,
            cancellationToken);
        return result.ToOkResponse();
    }

    private static async Task<IResult> GetIssueAsync(Guid id, BindableAuthData bindableAuthData,
        IssueService issueService, CancellationToken cancellationToken)
    {
        var result = await issueService.GetIssueAsync(id, bindableAuthData.UserId, cancellationToken);
        return result.ToOkResponse();
    }

    private static async Task<IResult> CreateIssueAsync(CreateIssueCommand command,
        BindableAuthData bindableAuthData, IssueService issueService, CancellationToken cancellationToken)
    {
        var result =
            await issueService.CreateIssueAsync(command, bindableAuthData.UserId, cancellationToken);
        return result.ToOkResponse();
    }

    private static async Task<IResult> UpdateIssueAsync(Guid id, UpdateIssueCommand command,
        BindableAuthData bindableAuthData, IssueService issueService, CancellationToken cancellationToken)
    {
        var result = await issueService.UpdateIssueAsync(id, command, bindableAuthData.UserId, cancellationToken);
        return result.ToOkResponse();
    }

    private static async Task<IResult> DeleteIssueAsync(Guid id, BindableAuthData bindableAuthData,
        IssueService issueService, CancellationToken cancellationToken)
    {
        var result = await issueService.DeleteIssueAsync(id, bindableAuthData.UserId, cancellationToken);
        return result.ToOkResponse();
    }
}