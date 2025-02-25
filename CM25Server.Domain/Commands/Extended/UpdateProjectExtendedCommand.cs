namespace CM25Server.Domain.Commands.Extended;

public record UpdateProjectExtendedCommand : UpdateProjectCommand
{
    public required Guid UserId { get; init; }
    public required Guid ProjectId { get; init; }
}