using CM25Server.Domain.Commands.Extended;
using CM25Server.Domain.Commands.Mappers;
using CM25Server.Domain.Commands.Validators;
using CM25Server.Domain.Core;
using CM25Server.Domain.Enums;

namespace CM25Server.Domain.Commands;

public record UpdateIssueCommand : BaseCommand<UpdateIssueCommand, UpdateIssueCommandValidator>
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public IssuePriority? Priority { get; init; }
    public IssueState? State { get; init; }
    public IssueStage? Stage { get; init; }
    
    public UpdateIssueExtendedCommand Extend(Guid issueId, Guid userId)
    {
        var mapper = new UpdateIssueCommandMapper();
        return mapper.ToExtendedCommand(this, issueId, userId);
    }
}