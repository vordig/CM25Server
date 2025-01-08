using CM25Server.Domain.Core;
using CM25Server.Domain.Enums;

namespace CM25Server.Domain.Models;

public class Issue : BaseModel
{
    public required Guid ProjectId { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public string Description { get; init; } = string.Empty;
    public IssuePriority Priority { get; init; } = IssuePriority.Normal;
    public IssueState State { get; init; } = IssueState.Unresolved;
}