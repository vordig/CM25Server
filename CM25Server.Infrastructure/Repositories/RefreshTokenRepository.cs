using CM25Server.Domain.Models;
using CM25Server.Infrastructure.Builders.Filter;
using CM25Server.Infrastructure.Core;
using CM25Server.Infrastructure.Core.Builders.Filter.Extensions;
using CM25Server.Infrastructure.Core.Repositories;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;

namespace CM25Server.Infrastructure.Repositories;

public class RefreshTokenRepository(DatabaseContext databaseContext, ILogger<ProjectRepository> logger)
    : BaseDatabaseRepository(databaseContext, "refresh-tokens", logger)
{
    public async Task<Result<RefreshToken>> CreateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        var refreshToken = new RefreshToken
        {
            UserId = userId,
        };
        var result = await DatabaseContext.CreateOneAsync(Collection, refreshToken, cancellationToken);
        return result.Match(
            _ => refreshToken,
            exception => new Result<RefreshToken>(exception)
        );
    }

    public async Task<Option<RefreshToken>> GetRefreshTokenAsync(Guid refreshTokenId,
        CancellationToken cancellationToken)
    {
        var filter = new RefreshTokenFilterBuilder()
            .WithId(refreshTokenId)
            .Build();

        return await DatabaseContext.GetOneAsync(Collection, filter, cancellationToken);
    }
    
    public async Task<Result<Guid>> DeleteRefreshTokenAsync(Guid refreshTokenId, CancellationToken cancellationToken)
    {
        var filter = new RefreshTokenFilterBuilder()
            .WithId(refreshTokenId)
            .Build();

        var result = await DatabaseContext.DeleteOneAsync(Collection, filter, cancellationToken);

        return result.Match(
            count => refreshTokenId,
            exception => new Result<Guid>(exception)
        );
    }
}