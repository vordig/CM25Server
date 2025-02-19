namespace CM25Server.Services.Contracts.Responses;

public record ProjectResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Code { get; init; }
    public required string Description { get; init; }
    public required DateTime ModifiedOn { get; init; }
}