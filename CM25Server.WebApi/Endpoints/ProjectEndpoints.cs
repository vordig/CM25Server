using CM25Server.Domain.Commands;
using CM25Server.Infrastructure.Enums;
using CM25Server.Services;
using CM25Server.Services.Contracts.Responses;
using CM25Server.Services.Core;
using CM25Server.WebApi.ApiVersioning;
using CM25Server.WebApi.Data;
using CM25Server.WebApi.Extensions;

namespace CM25Server.WebApi.Endpoints;

public static class ProjectEndpoints
{
    private const string AnyProjectEndpointName = "AnyProject";
    private const string GetProjectsEndpointName = "GetProjects";
    private const string GetProjectEndpointName = "GetProject";
    private const string CreateProjectEndpointName = "CreateProject";
    private const string UpdateProjectEndpointName = "UpdateProject";
    private const string DeleteProjectEndpointName = "DeleteProject";

    public static IEndpointRouteBuilder MapProjectEndpoints(this IEndpointRouteBuilder endpoints,
        ApiVersionManager apiVersionManager)
    {
        var api = apiVersionManager.GetApi(endpoints, "projects", "Projects", ApiVersionManager.ApiVersions.V1_0);

        api.MapProjectEndpoints();

        return endpoints;
    }

    private static void MapProjectEndpoints(this IEndpointRouteBuilder api)
    {
        api
            .MapGet("any", AnyProjectAsync)
            .WithName(AnyProjectEndpointName)
            .RequireAuthorization()
            .Produces<bool>();

        api
            .MapGet("", GetProjectsAsync)
            .WithName(GetProjectsEndpointName)
            .RequireAuthorization()
            .Produces<PageResponse<ProjectResponse>>();

        api
            .MapGet("{id:guid}", GetProjectAsync)
            .WithName(GetProjectEndpointName)
            .RequireAuthorization()
            .Produces<ProjectResponse>();

        api
            .MapPost("", CreateProjectAsync)
            .WithName(CreateProjectEndpointName)
            .RequireAuthorization()
            .Produces<ProjectResponse>();

        api
            .MapPut("{id:guid}", UpdateProjectAsync)
            .WithName(UpdateProjectEndpointName)
            .RequireAuthorization()
            .Produces<Guid>();

        api
            .MapDelete("{id:guid}", DeleteProjectAsync)
            .WithName(DeleteProjectEndpointName)
            .RequireAuthorization()
            .Produces<Guid>();
    }

    private static async Task<IResult> AnyProjectAsync(AuthData authData, ProjectService projectService,
        CancellationToken cancellationToken)
    {
        var result = await projectService.AnyProjectAsync(authData.UserId, cancellationToken);
        return result.ToOkResponse();
    }

    private static async Task<IResult> GetProjectsAsync(BindableProjectFilteringData filteringData,
        BindableSortingData<ProjectSortBy> sortingData, BindablePagingData pagingData, AuthData authData,
        ProjectService projectService, CancellationToken cancellationToken)
    {
        var result = await projectService.GetProjectsAsync(filteringData, sortingData, pagingData, authData.UserId,
            cancellationToken);
        return result.ToOkResponse();
    }

    private static async Task<IResult> GetProjectAsync(Guid id, AuthData authData, ProjectService projectService,
        CancellationToken cancellationToken)
    {
        var result = await projectService.GetProjectAsync(id, authData.UserId, cancellationToken);
        return result.ToOkResponse();
    }

    private static async Task<IResult> CreateProjectAsync(CreateProjectCommand command, AuthData authData,
        ProjectService projectService, CancellationToken cancellationToken)
    {
        var result = await projectService.CreateProjectAsync(command, authData.UserId, cancellationToken);
        return result.ToOkResponse();
    }

    private static async Task<IResult> UpdateProjectAsync(Guid id, UpdateProjectCommand command, AuthData authData,
        ProjectService projectService, CancellationToken cancellationToken)
    {
        var result = await projectService.UpdateProjectAsync(id, command, authData.UserId, cancellationToken);
        return result.ToOkResponse();
    }

    private static async Task<IResult> DeleteProjectAsync(Guid id, AuthData authData, ProjectService projectService,
        CancellationToken cancellationToken)
    {
        var result = await projectService.DeleteProjectAsync(id, authData.UserId, cancellationToken);
        return result.ToOkResponse();
    }
}