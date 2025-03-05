using CM25Server.Domain.Commands.Extended;
using CM25Server.Domain.Commands.Mappers;
using CM25Server.Domain.Core;
using CM25Server.Domain.Enums;

namespace CM25Server.Domain.Models;

public class Issue : BaseOwnedByUserModel
{
    public Guid ProjectId { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public IssuePriority Priority { get; set; } = IssuePriority.Normal;
    public IssueState State { get; set; } = IssueState.Open;
    public IssueStage Stage { get; set; } = IssueStage.Backlog;
    
    public static Issue FromCommand(CreateIssueExtendedCommand command)
    {
        var mapper = new CreateIssueExtendedCommandMapper();
        return mapper.ToIssue(command);
    }
}