using CM25Server.Domain.Models;
using CM25Server.Services;
using CM25Server.Services.Contracts;
using CM25Server.Services.Contracts.Requests;
using CM25Server.WebApi.ApiVersioning;

namespace CM25Server.WebApi.Endpoints;

public static class IssueEndpoints
{
    private const string GetIssuesEndpointName = "GetIssues";
    private const string GetIssueEndpointName = "GetIssue";
    private const string CreateIssueEndpointName = "CreateIssue";
    private const string UpdateIssueEndpointName = "UpdateIssue";
    private const string DeleteIssueEndpointName = "DeleteIssue";
    private const string GenerateIssuesEndpointName = "GenerateIssues";
    private const string ClearIssuesEndpointName = "ClearIssues";

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
            .MapGet("{projectId:guid}", GetIssuesAsync)
            .WithName(GetIssuesEndpointName)
            .Produces<Issue[]>();
        
        api
            .MapGet("{projectId:guid}/{id:guid}", GetIssueAsync)
            .WithName(GetIssueEndpointName)
            .Produces<Issue>();
        
        api
            .MapPost("{projectId:guid}", CreateIssueAsync)
            .WithName(CreateIssueEndpointName)
            .Produces<Issue>();
        
        api
            .MapPut("{projectId:guid}/{id:guid}", UpdateIssueAsync)
            .WithName(UpdateIssueEndpointName)
            .Produces<Issue>();
        
        api
            .MapDelete("{projectId:guid}/{id:guid}", DeleteIssueAsync)
            .WithName(DeleteIssueEndpointName)
            .Produces<bool>();
        
        api
            .MapPost("{projectId:guid}/generate/{count:int}", GenerateIssuesAsync)
            .WithName(GenerateIssuesEndpointName)
            .Produces<int>();
        
        api
            .MapDelete("{projectId:guid}/all", ClearIssuesAsync)
            .WithName(ClearIssuesEndpointName)
            .Produces<int>();
    }

    private static IResult GetIssuesAsync(Guid projectId, IssueService issueService, CancellationToken cancellationToken)
    {
        var result = issueService.GetIssues(projectId);

        return result.Match(
            Results.Ok,
            _ => Results.NotFound()
        );
    }
    
    private static IResult GetIssueAsync(Guid id, Guid projectId, IssueService issueService, CancellationToken cancellationToken)
    {
        var result = issueService.GetIssue(id, projectId);
        
        return result.Match(
            Results.Ok,
            Results.NotFound()
        );
    }
    
    private static IResult CreateIssueAsync(Guid projectId, IssueRequest request, IssueService issueService, CancellationToken cancellationToken)
    {
        var result = issueService.CreateIssue(request, projectId);

        return result.Match(
            Results.Ok,
            Results.InternalServerError
        );
    }
    
    private static IResult UpdateIssueAsync(Guid id, Guid projectId, IssueRequest request, IssueService issueService, CancellationToken cancellationToken)
    {
        var result = issueService.UpdateIssue(id, request, projectId);

        return result.Match(
            Results.Ok,
            Results.InternalServerError
        );
    }
    
    private static IResult DeleteIssueAsync(Guid id, Guid projectId, IssueService issueService, CancellationToken cancellationToken)
    {
        var result = issueService.DeleteIssue(id, projectId);

        return result.Match(
            Results.Ok,
            Results.InternalServerError
        );
    }
    
    private static IResult GenerateIssuesAsync(int count, Guid projectId, IssueService issueService, CancellationToken cancellationToken)
    {
        var result = issueService.GenerateIssues(count, projectId);

        return result.Match(
            Results.Ok,
            Results.InternalServerError
        );
    }
    
    private static IResult ClearIssuesAsync(Guid projectId, IssueService issueService, CancellationToken cancellationToken)
    {
        var result = issueService.ClearIssues(projectId);

        return result.Match(
            Results.Ok,
            Results.InternalServerError
        );
    }
}