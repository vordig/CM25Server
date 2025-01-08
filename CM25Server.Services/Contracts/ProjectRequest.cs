namespace CM25Server.Services.Contracts;

public record ProjectRequest
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public string Description { get; init; } = string.Empty;
}