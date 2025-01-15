using CM25Server.Domain.Core;
using CM25Server.Domain.Enums;

namespace CM25Server.Domain.Models;

public class Issue : BaseModel
{
    public required string Name { get; set; }
    public Guid ProjectId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IssuePriority Priority { get; set; } = IssuePriority.Normal;
    public IssueState State { get; set; } = IssueState.Unresolved;
}