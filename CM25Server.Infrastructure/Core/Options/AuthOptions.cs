namespace CM25Server.Infrastructure.Core.Options;

public record AuthOptions
{
    public const string SectionName = "AuthOptions";

    public required string JWTSecret { get; init; }
    public required string JWTIssuer { get; init; }
    public required string JWTAudience { get; init; }
    public required int JWTTimeToLive { get; init; }
    public required int RefreshTokenTimeToLive { get; init; }
}