namespace CM25Server.Domain.Commands.Extended;

public record CreateProjectExtendedCommand : CreateProjectCommand
{
    public required Guid UserId { get; init; }
}