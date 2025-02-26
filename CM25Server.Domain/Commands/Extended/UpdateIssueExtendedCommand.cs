namespace CM25Server.Domain.Commands.Extended;

public record UpdateIssueExtendedCommand : UpdateIssueCommand
{
    public required Guid UserId { get; init; }
    public required Guid IssueId { get; init; }
}