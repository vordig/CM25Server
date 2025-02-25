using CM25Server.Domain.Commands;
using CM25Server.Domain.Commands.Extended;
using CM25Server.Domain.Exceptions;
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

public class IssueService(
    ProjectRepository projectRepository,
    IssueRepository issueRepository,
    ILogger<IssueService> logger)
{
    public async Task<Result<IssueListResponse>> CreateIssueAsync(CreateIssueCommand command, Guid projectId,
        Guid userId, CancellationToken cancellationToken)
    {
        var commandValidationResult = await command.ValidateAsync(cancellationToken);
        return await commandValidationResult.Match(
            async validatedCommand =>
                await ValidatedCreateIssueAsync(validatedCommand, projectId, userId, cancellationToken),
            exception => Task.FromResult(new Result<IssueListResponse>(exception))
        );
    }

    public async Task<Option<IssueDetailResponse>> GetIssueAsync(Guid issueId, Guid projectId, Guid userId,
        CancellationToken cancellationToken)
    {
        var issueResult = await issueRepository.GetIssueAsync(issueId, projectId, userId, cancellationToken);

        return issueResult.Match(
            issue =>
            {
                var mapper = new IssueMapper();
                return mapper.ToDetailResponse(issue);
            },
            Option<IssueDetailResponse>.None
        );
    }

    public async Task<Result<PageResponse<IssueListResponse>>> GetIssuesAsync(IssueFilteringData filteringData,
        SortingData<IssueSortBy> sortingData, PagingData pagingData, Guid projectId, Guid userId,
        CancellationToken cancellationToken)
    {
        var itemsTask = issueRepository.GetIssuesAsync(filteringData, sortingData, pagingData, projectId, userId,
            cancellationToken);
        var itemsCountTask = issueRepository.CountAsync(filteringData, projectId, userId, cancellationToken);

        await Task.WhenAll(itemsTask, itemsCountTask);

        var items = itemsTask.Result.IfNone([]);
        var itemsCount = itemsCountTask.Result.IfFail(0);

        var mapper = new IssueMapper();
        var itemsResponse = items.Select(mapper.ToListResponse).ToArray();

        var result = new PageResponse<IssueListResponse>
        {
            Items = itemsResponse,
            Total = itemsCount,
            PageNumber = pagingData.PageNumber,
            PageSize = pagingData.PageSize
        };

        return result;
    }

    public async Task<Result<Guid>> UpdateIssueAsync(Guid issueId, UpdateIssueCommand command, Guid projectId,
        Guid userId, CancellationToken cancellationToken)
    {
        var commandValidationResult = await command.ValidateAsync(cancellationToken);
        return await commandValidationResult.Match(
            async validatedCommand =>
                await ValidatedUpdateIssueAsync(issueId, validatedCommand, projectId, userId, cancellationToken),
            exception => Task.FromResult(new Result<Guid>(exception))
        );
    }

    public async Task<Result<bool>> AnyIssueAsync(Guid projectId, Guid userId, CancellationToken cancellationToken)
    {
        return await issueRepository.AnyAsync(projectId, userId, cancellationToken);
    }

    public async Task<Result<Guid>> DeleteIssueAsync(Guid issueId, Guid projectId, Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await issueRepository.DeleteIssueAsync(issueId, projectId, userId, cancellationToken);

        return result.Match(
            updatedIssueId =>
            {
                logger.LogInformation("Issue {IssueId} deleted", updatedIssueId);
                return updatedIssueId;
            },
            exception =>
            {
                logger.LogError(exception, "Issue {IssueId} not deleted", issueId);
                return new Result<Guid>(exception);
            }
        );
    }

    private async Task<Result<IssueListResponse>> ValidatedCreateIssueAsync(CreateIssueCommand command, Guid projectId,
        Guid userId, CancellationToken cancellationToken)
    {
        var projectResult = await projectRepository.GetProjectAsync(projectId, userId, cancellationToken);
        return await projectResult.MatchAsync(async project =>
            {
                var code = project.Code;
                var extendedCommand = command.Extend(code, projectId, userId);
                return await CreateIssueAsync(extendedCommand, cancellationToken);
            },
            () => new Result<IssueListResponse>(new ProblemException("Project not found", "Project not found"))
        );
    }

    private async Task<Result<IssueListResponse>> CreateIssueAsync(CreateIssueExtendedCommand command,
        CancellationToken cancellationToken)
    {
        var result = await issueRepository.CreateIssueAsync(command, cancellationToken);
        return result.Match(
            createdIssue =>
            {
                logger.LogInformation("Issue {Name} with Id {IssueId} created", createdIssue.Name,
                    createdIssue.Id);
                var mapper = new IssueMapper();
                return mapper.ToListResponse(createdIssue);
            },
            exception =>
            {
                logger.LogError(exception, "Issue {Name} not created", command.Name);
                return new Result<IssueListResponse>(exception);
            }
        );
    }

    private async Task<Result<Guid>> ValidatedUpdateIssueAsync(Guid issueId, UpdateIssueCommand command,
        Guid projectId, Guid userId, CancellationToken cancellationToken)
    {
        var extendedCommand = command.Extend(issueId, projectId, userId);
        var result = await issueRepository.UpdateIssueAsync(extendedCommand, cancellationToken);
        return result.Match(
            updatedIssueId =>
            {
                logger.LogInformation("Issue {IssueId} updated", updatedIssueId);
                return updatedIssueId;
            },
            exception =>
            {
                logger.LogError(exception, "Issue {IssueId} not updated", issueId);
                return new Result<Guid>(exception);
            }
        );
    }
}