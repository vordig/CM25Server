using CM25Server.Domain.Enums;

namespace CM25Server.Services.Contracts;

public record IssueListResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Code { get; init; }
    public required IssuePriority Priority { get; init; }
    public required IssueState State { get; init; }
    public required DateTime CreatedOn { get; init; }
    public required DateTime ModifiedOn { get; init; }
}