using CM25Server.Domain.Commands;
using CM25Server.Infrastructure.Core.Data;
using CM25Server.Infrastructure.Data;
using CM25Server.Infrastructure.Enums;
using CM25Server.Infrastructure.Repositories;
using CM25Server.Services.Contracts;
using CM25Server.Services.Core;
using CM25Server.Services.Mappers;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;

namespace CM25Server.Services;

public class ProjectService(ProjectRepository projectRepository, ILogger<ProjectService> logger)
{
    public async Task<Result<ProjectListResponse>> CreateProjectAsync(CreateProjectCommand command, Guid userId,
        CancellationToken cancellationToken)
    {
        var commandValidationResult = await command.ValidateAsync(cancellationToken);
        return await commandValidationResult.Match(
            async validatedCommand => await ValidatedCreateProjectAsync(validatedCommand, userId, cancellationToken),
            exception => Task.FromResult(new Result<ProjectListResponse>(exception))
        );
    }

    public async Task<Option<ProjectDetailResponse>> GetProjectAsync(Guid projectId, Guid userId,
        CancellationToken cancellationToken)
    {
        var projectResult = await projectRepository.GetProjectAsync(projectId, userId, cancellationToken);

        return projectResult.Match(
            project =>
            {
                var mapper = new ProjectMapper();
                return mapper.ToDetailResponse(project);
            },
            Option<ProjectDetailResponse>.None
        );
    }

    public async Task<Result<PageResponse<ProjectListResponse>>> GetProjectsAsync(ProjectFilteringData filteringData,
        SortingData<ProjectSortBy> sortingData, PagingData pagingData, Guid userId, CancellationToken cancellationToken)
    {
        var itemsTask =
            projectRepository.GetProjectsAsync(filteringData, sortingData, pagingData, userId, cancellationToken);
        var itemsCountTask = projectRepository.CountAsync(filteringData, userId, cancellationToken);

        await Task.WhenAll(itemsTask, itemsCountTask);

        var items = itemsTask.Result.IfNone([]);
        var itemsCount = itemsCountTask.Result.IfFail(0);

        var mapper = new ProjectMapper();
        var itemsResponse = items.Select(mapper.ToListResponse).ToArray();

        var result = new PageResponse<ProjectListResponse>
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
        var commandValidationResult = await command.ValidateAsync(cancellationToken);
        return await commandValidationResult.Match(
            async validatedCommand =>
                await ValidatedUpdateProjectAsync(projectId, validatedCommand, userId, cancellationToken),
            exception => Task.FromResult(new Result<Guid>(exception))
        );
    }

    public async Task<Result<bool>> AnyProjectAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await projectRepository.AnyAsync(userId, cancellationToken);
    }

    public async Task<Result<Guid>> DeleteProjectAsync(Guid projectId, Guid userId, CancellationToken cancellationToken)
    {
        var result = await projectRepository.DeleteProjectAsync(projectId, userId, cancellationToken);

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

    private async Task<Result<ProjectListResponse>> ValidatedCreateProjectAsync(CreateProjectCommand command, Guid userId,
        CancellationToken cancellationToken)
    {
        var extendedCommand = command.Extend(userId);
        var result = await projectRepository.CreateProjectAsync(extendedCommand, cancellationToken);
        return result.Match(
            createdProject =>
            {
                logger.LogInformation("Project {Name} with Id {ProjectId} created", createdProject.Name,
                    createdProject.Id);
                var mapper = new ProjectMapper();
                return mapper.ToListResponse(createdProject);
            },
            exception =>
            {
                logger.LogError(exception, "Project {Name} not created", command.Name);
                return new Result<ProjectListResponse>(exception);
            }
        );
    }

    private async Task<Result<Guid>> ValidatedUpdateProjectAsync(Guid projectId, UpdateProjectCommand command,
        Guid userId, CancellationToken cancellationToken)
    {
        var extendedCommand = command.Extend(projectId, userId);
        var result = await projectRepository.UpdateProjectAsync(extendedCommand, cancellationToken);
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
}