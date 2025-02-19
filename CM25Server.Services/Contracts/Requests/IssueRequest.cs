using CM25Server.Domain.Enums;

namespace CM25Server.Services.Contracts.Requests;

public record IssueRequest
{
    public required string Name { get; init; }
    public string Description { get; init; } = string.Empty;
    public IssuePriority Priority { get; init; } = IssuePriority.Normal;
    public IssueState State { get; init; } = IssueState.Unresolved;
}