using CM25Server.Domain.Enums;

namespace CM25Server.Domain.Commands.Extended;

public record CreateIssueExtendedCommand : CreateIssueCommand
{
    public required Guid UserId { get; init; }
    public required Guid ProjectId { get; init; }
    public required string Code { get; init; }
    public required IssueState State { get; init; }
}