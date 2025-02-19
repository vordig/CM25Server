namespace CM25Server.Domain.Commands;

public record CreateProjectCommand
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public string Description { get; init; } = string.Empty;
}