namespace CM25Server.Services.Contracts;

public record ProjectListResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Code { get; init; }
    public required DateTime CreatedOn { get; init; }
    public required DateTime ModifiedOn { get; init; }
}