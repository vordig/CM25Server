namespace CM25Server.Infrastructure.Core.Options;

public record AuthOptions
{
    public const string SectionName = "AuthOptions";

    public required string AccessTokenSecret { get; init; }
    public required string AccessTokenIssuer { get; init; }
    public required string AccessTokenAudience { get; init; }
    public required int AccessTokenTimeToLive { get; init; }
    public required string RefreshTokenSecret { get; init; }
    public required string RefreshTokenIssuer { get; init; }
    public required string RefreshTokenAudience { get; init; }
    public required int RefreshTokenTimeToLive { get; init; }
}