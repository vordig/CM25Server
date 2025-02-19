namespace CM25Server.Services.Core;

public record AuthResponse
{
    public required string AccessToken { get; init; }
}