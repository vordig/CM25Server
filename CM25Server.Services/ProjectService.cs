using CM25Server.Domain.Commands;
using CM25Server.Infrastructure.Core.Data;
using CM25Server.Infrastructure.Data;
using CM25Server.Infrastructure.Enums;
using CM25Server.Infrastructure.Repositories;
using CM25Server.Services.Contracts.Responses;
using CM25Server.Services.Core;
using CM25Server.Services.Mappers;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;

namespace CM25Server.Services;

public class ProjectService(ProjectRepository projectRepository, ILogger<ProjectService> logger)
{
    public async Task<Result<ProjectResponse>> CreateProjectAsync(CreateProjectCommand command, Guid userId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("CreateProjectAsync requested by {UserId}", userId);
        
        // todo: add validator
        
        var result = await projectRepository.CreateProjectAsync(command, cancellationToken);
        return result.Match(
            createdProject =>
            {
                logger.LogInformation("Project {Name} with Id {ProjectId} created", createdProject.Name,
                    createdProject.Id);
                var mapper = new ProjectMapper();
                return mapper.ToResponse(createdProject);
            },
            exception =>
            {
                logger.LogError(exception, "Project {Name} not created", command.Name);
                return new Result<ProjectResponse>(exception);
            }
        );
    }

    public async Task<Option<ProjectResponse>> GetProjectAsync(Guid projectId, Guid userId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("GetProjectAsync requested by {UserId}", userId);
        
        var projectResult = await projectRepository.GetProjectAsync(projectId, cancellationToken);

        return projectResult.Match(
            project =>
            {
                var mapper = new ProjectMapper();
                return mapper.ToResponse(project);
            },
            Option<ProjectResponse>.None
        );
    }

    public async Task<Result<PageResponse<ProjectResponse>>> GetProjectsAsync(ProjectFilteringData filteringData,
        SortingData<ProjectSortBy> sortingData, PagingData pagingData, Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation("GetProjectsAsync requested by {UserId}", userId);
        
        var itemsTask = projectRepository.GetProjectsAsync(filteringData, sortingData, pagingData, cancellationToken);
        var itemsCountTask = projectRepository.CountAsync(filteringData, cancellationToken);

        await Task.WhenAll(itemsTask, itemsCountTask);

        var items = itemsTask.Result.IfNone([]);
        var itemsCount = itemsCountTask.Result.IfFail(0);

        var mapper = new ProjectMapper();
        var itemsResponse = items.Select(mapper.ToResponse).ToArray();

        var result = new PageResponse<ProjectResponse>
        {
            Items = itemsResponse,
            Total = itemsCount,
            PageNumber = pagingData.PageNumber,
            PageSize = pagingData.PageSize
        };

        return result;
    }

    public async Task<Result<Guid>> UpdateProjectAsync(Guid projectId, UpdateProjectCommand command, Guid userId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("UpdateProjectAsync requested by {UserId}", userId);
        
        // todo: add validator

        var result = await projectRepository.UpdateProjectAsync(projectId, command, cancellationToken);
        return result.Match(
            updatedProjectId =>
            {
                logger.LogInformation("Project {ProjectId} updated", updatedProjectId);
                return updatedProjectId;
            },
            exception =>
            {
                logger.LogError(exception, "Project {ProjectId} not updated", projectId);
                return new Result<Guid>(exception);
            }
        );
    }

    public async Task<Result<bool>> AnyProjectAsync(Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation("AnyProjectAsync requested by {UserId}", userId);
        return await projectRepository.AnyAsync(cancellationToken);
    }

    public async Task<Result<Guid>> DeleteProjectAsync(Guid projectId, Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation("DeleteProjectAsync requested by {UserId}", userId);
        
        var result = await projectRepository.DeleteProjectAsync(projectId, cancellationToken);

        return result.Match(
            updatedProjectId =>
            {
                logger.LogInformation("Project {ProjectId} deleted", updatedProjectId);
                return updatedProjectId;
            },
            exception =>
            {
                logger.LogError(exception, "Project {ProjectId} not deleted", projectId);
                return new Result<Guid>(exception);
            }
        );
    }
}