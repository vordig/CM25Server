using CM25Server.Domain.Commands.Extended;
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

public class IssueRepository(DatabaseContext databaseContext, ILogger<IssueRepository> logger)
    : BaseDatabaseRepository(databaseContext, "issues", logger)
{
    public async Task<Result<long>> CountAsync(IssueFilteringData filteringData, Guid userId,
        CancellationToken cancellationToken)
    {
        var filter = new IssueFilterBuilder()
            .OwnedByUser(userId)
            .ForProjects(filteringData.ProjectIds)
            .WithState(filteringData.State)
            .WithPriorities(filteringData.Priorities)
            .SearchFor(filteringData.SearchTerm)
            .Build();

        return await DatabaseContext.CountAsync(Collection, filter, cancellationToken);
    }

    public async Task<Result<Issue>> CreateIssueAsync(CreateIssueExtendedCommand command,
        CancellationToken cancellationToken)
    {
        var issue = Issue.FromCommand(command);
        var result = await DatabaseContext.CreateOneAsync(Collection, issue, cancellationToken);
        return result.Match(
            _ => issue,
            exception => new Result<Issue>(exception)
        );
    }

    public async Task<Option<Issue>> GetIssueAsync(Guid issueId, Guid userId,
        CancellationToken cancellationToken)
    {
        var filter = new IssueFilterBuilder()
            .WithId(issueId)
            .OwnedByUser(userId)
            .Build();

        return await DatabaseContext.GetOneAsync(Collection, filter, cancellationToken);
    }
    
    public async Task<Option<IReadOnlyCollection<Issue>>> GetIssuesAsync(IssueFilteringData filteringData,
        SortingData<IssueSortBy> sortingData, PagingData pagingData, Guid userId, CancellationToken cancellationToken)
    {
        var filter = new IssueFilterBuilder()
            .OwnedByUser(userId)
            .ForProjects(filteringData.ProjectIds)
            .WithState(filteringData.State)
            .WithPriorities(filteringData.Priorities)
            .SearchFor(filteringData.SearchTerm)
            .Build();

        var sort = new IssueSortBuilder()
            .SortBy(sortingData)
            .Build();

        return await DatabaseContext.GetPageAsync(Collection, filter, sort, pagingData.PageNumber, pagingData.PageSize,
            cancellationToken);
    }

    public async Task<Result<Guid>> UpdateIssueAsync(UpdateIssueExtendedCommand command,
        CancellationToken cancellationToken)
    {
        var filter = new IssueFilterBuilder()
            .WithId(command.IssueId)
            .OwnedByUser(command.UserId)
            .Build();

        var update = new IssueUpdateBuilder()
            .FromCommand(command)
            .UpdateModifiedOn()
            .Build();

        var result = await DatabaseContext.UpdateOneAsync(Collection, filter, update, cancellationToken);

        return result.Match(
            count => command.IssueId,
            exception => new Result<Guid>(exception)
        );
    }

    public async Task<Result<Guid>> DeleteIssueAsync(Guid issueId, Guid userId, CancellationToken cancellationToken)
    {
        var filter = new IssueFilterBuilder()
            .WithId(issueId)
            .OwnedByUser(userId)
            .Build();

        var result = await DatabaseContext.DeleteOneAsync(Collection, filter, cancellationToken);

        return result.Match(
            count => issueId,
            exception => new Result<Guid>(exception)
        );
    }
}