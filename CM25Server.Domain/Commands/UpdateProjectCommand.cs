namespace CM25Server.Domain.Commands;

public record UpdateProjectCommand
{
    public string? Code { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
}