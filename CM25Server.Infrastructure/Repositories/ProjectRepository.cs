using CM25Server.Domain.Commands;
using CM25Server.Domain.Models;
using CM25Server.Infrastructure.Builders.Filter;
using CM25Server.Infrastructure.Builders.Sort;
using CM25Server.Infrastructure.Builders.Update;
using CM25Server.Infrastructure.Core;
using CM25Server.Infrastructure.Core.Builders.Filter.Extensions;
using CM25Server.Infrastructure.Core.Builders.Sort.Extensions;
using CM25Server.Infrastructure.Core.Builders.Update.Extensions;
using CM25Server.Infrastructure.Core.Data;
using CM25Server.Infrastructure.Core.Repositories;
using CM25Server.Infrastructure.Data;
using CM25Server.Infrastructure.Enums;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;

namespace CM25Server.Infrastructure.Repositories;

public class ProjectRepository(DatabaseContext databaseContext, ILogger<ProjectRepository> logger)
    : BaseDatabaseRepository(databaseContext, "projects", logger)
{
    public async Task<Result<bool>> AnyAsync(Guid userId, CancellationToken cancellationToken)
    {
        var filter = new ProjectFilterBuilder()
            .OwnedByUser(userId)
            .Build();

        var result = await DatabaseContext.AnyAsync(Collection, filter, cancellationToken);

        return result;
    }

    public async Task<Result<long>> CountAsync(ProjectFilteringData filteringData, Guid userId,
        CancellationToken cancellationToken)
    {
        var filter = new ProjectFilterBuilder()
            .OwnedByUser(userId)
            .SearchFor(filteringData.SearchTerm)
            .Build();

        return await DatabaseContext.CountAsync(Collection, filter, cancellationToken);
    }

    public async Task<Result<Project>> CreateProjectAsync(CreateProjectCommand command, Guid userId,
        CancellationToken cancellationToken)
    {
        var project = Project.FromCommand(command, userId);
        var result = await DatabaseContext.CreateOneAsync(Collection, project, cancellationToken);
        return result.Match(
            _ => project,
            exception => new Result<Project>(exception)
        );
    }

    public async Task<Option<Project>> GetProjectAsync(Guid projectId, Guid userId, CancellationToken cancellationToken)
    {
        var filter = new ProjectFilterBuilder()
            .WithId(projectId)
            .OwnedByUser(userId)
            .Build();

        return await DatabaseContext.GetOneAsync(Collection, filter, cancellationToken);
    }

    public async Task<Option<IReadOnlyCollection<Project>>> GetProjectsAsync(ProjectFilteringData filteringData,
        SortingData<ProjectSortBy> sortingData, PagingData pagingData, Guid userId, CancellationToken cancellationToken)
    {
        var filter = new ProjectFilterBuilder()
            .OwnedByUser(userId)
            .SearchFor(filteringData.SearchTerm)
            .Build();

        var sort = new ProjectSortBuilder()
            .SortBy(sortingData)
            .Build();

        return await DatabaseContext.GetPageAsync(Collection, filter, sort, pagingData.PageNumber, pagingData.PageSize,
            cancellationToken);
    }

    public async Task<Result<Guid>> UpdateProjectAsync(Guid projectId, UpdateProjectCommand command, Guid userId,
        CancellationToken cancellationToken)
    {
        var filter = new ProjectFilterBuilder()
            .WithId(projectId)
            .OwnedByUser(userId)
            .Build();

        var update = new ProjectUpdateBuilder()
            .FromCommand(command)
            .UpdateModifiedOn()
            .Build();

        var result = await DatabaseContext.UpdateOneAsync(Collection, filter, update, cancellationToken);

        return result.Match(
            count => projectId,
            exception => new Result<Guid>(exception)
        );
    }

    public async Task<Result<Guid>> DeleteProjectAsync(Guid projectId, Guid userId, CancellationToken cancellationToken)
    {
        var filter = new ProjectFilterBuilder()
            .WithId(projectId)
            .OwnedByUser(userId)
            .Build();

        var result = await DatabaseContext.DeleteOneAsync(Collection, filter, cancellationToken);

        return result.Match(
            count => projectId,
            exception => new Result<Guid>(exception)
        );
    }
}