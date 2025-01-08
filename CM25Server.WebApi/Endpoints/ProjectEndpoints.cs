using CM25Server.Domain.Models;
using CM25Server.Services;
using CM25Server.Services.Contracts;
using CM25Server.WebApi.ApiVersioning;

namespace CM25Server.WebApi.Endpoints;

public static class ProjectEndpoints
{
    private const string GetProjectsEndpointName = "GetProjects";
    private const string GetProjectEndpointName = "GetProject";
    private const string CreateProjectEndpointName = "CreateProject";
    private const string UpdateProjectEndpointName = "UpdateProject";
    private const string DeleteProjectEndpointName = "DeleteProject";
    private const string GenerateProjectsEndpointName = "GenerateProjects";

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
            .MapGet("", GetProjectsAsync)
            .WithName(GetProjectsEndpointName)
            .Produces<Project[]>();
        
        api
            .MapGet("{id:guid}", GetProjectAsync)
            .WithName(GetProjectEndpointName)
            .Produces<Project>();
        
        api
            .MapPost("", CreateProjectAsync)
            .WithName(CreateProjectEndpointName)
            .Produces<Project>();
        
        api
            .MapPut("{id:guid}", UpdateProjectAsync)
            .WithName(UpdateProjectEndpointName)
            .Produces<Project>();
        
        api
            .MapDelete("{id:guid}", DeleteProjectAsync)
            .WithName(DeleteProjectEndpointName)
            .Produces<bool>();
        
        api
            .MapGet("generate/{count:int}", GenerateProjectsAsync)
            .WithName(GenerateProjectsEndpointName)
            .Produces<int>();
    }

    private static IResult GetProjectsAsync(ProjectService projectService, CancellationToken cancellationToken)
    {
        var result = projectService.GetProjects();

        return result.Match(
            Results.Ok,
            _ => Results.NotFound()
        );
    }
    
    private static IResult GetProjectAsync(Guid id, ProjectService projectService, CancellationToken cancellationToken)
    {
        var result = projectService.GetProject(id);
        
        return result.Match(
            Results.Ok,
            Results.NotFound()
        );
    }
    
    private static IResult CreateProjectAsync(ProjectRequest request, ProjectService projectService, CancellationToken cancellationToken)
    {
        var result = projectService.CreateProject(request);

        return result.Match(
            Results.Ok,
            _ => Results.InternalServerError()
        );
    }
    
    private static IResult UpdateProjectAsync(Guid id, ProjectRequest request, ProjectService projectService, CancellationToken cancellationToken)
    {
        var result = projectService.UpdateProject(id, request);

        return result.Match(
            Results.Ok,
            Results.NotFound()
        );
    }
    
    private static IResult DeleteProjectAsync(Guid id, ProjectService projectService, CancellationToken cancellationToken)
    {
        var result = projectService.DeleteProject(id);

        return result.Match(
            Results.Ok,
            _ => Results.InternalServerError()
        );
    }
    
    private static IResult GenerateProjectsAsync(int count, ProjectService projectService, CancellationToken cancellationToken)
    {
        var result = projectService.GenerateProjects(count);

        return result.Match(
            Results.Ok,
            _ => Results.NotFound()
        );
    }
}