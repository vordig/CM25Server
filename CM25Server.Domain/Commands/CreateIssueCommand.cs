using CM25Server.Domain.Commands.Extended;
using CM25Server.Domain.Commands.Mappers;
using CM25Server.Domain.Commands.Validators;
using CM25Server.Domain.Core;
using CM25Server.Domain.Enums;

namespace CM25Server.Domain.Commands;

public record CreateIssueCommand : BaseCommand<CreateIssueCommand, CreateIssueCommandValidator>
{
    public required string Name { get; init; }
    public string Description { get; init; } = string.Empty;
    public IssuePriority Priority { get; init; } = IssuePriority.Normal;
    
    public CreateIssueExtendedCommand Extend(string code, Guid projectId, Guid userId)
    {
        var mapper = new CreateIssueCommandMapper();
        return mapper.ToExtendedCommand(this, code, projectId, userId);
    }
}