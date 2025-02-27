namespace CM25Server.Domain.Commands;

public record RefreshCommand
{
    public required string Token { get; init; }
}