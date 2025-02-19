namespace CM25Server.Domain.Commands;

public record LoginCommand
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}