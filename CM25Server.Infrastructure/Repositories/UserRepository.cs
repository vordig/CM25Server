using CM25Server.Domain.Commands;
using CM25Server.Domain.Models;
using CM25Server.Infrastructure.Builders.Filter;
using CM25Server.Infrastructure.Core;
using CM25Server.Infrastructure.Core.Builders.Filter.Extensions;
using CM25Server.Infrastructure.Core.Repositories;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;

namespace CM25Server.Infrastructure.Repositories;

public class UserRepository(DatabaseContext databaseContext, ILogger<UserRepository> logger)
    : BaseDatabaseRepository(databaseContext, "users", logger)
{
    public async Task<Result<bool>> AnyAsync(string email, CancellationToken cancellationToken)
    {
        var filter = new UserFilterBuilder()
            .WithEmail(email)
            .Build();

        var result = await DatabaseContext.AnyAsync(Collection, filter, cancellationToken);

        return result;
    }
    
    public async Task<Result<User>> CreateUserAsync(SignUpCommand command, CancellationToken cancellationToken)
    {
        var user = User.FromCommand(command);
        var result = await DatabaseContext.CreateOneAsync(Collection, user, cancellationToken);
        return result.Match(
            _ => user,
            exception => new Result<User>(exception)
        );
    }
    
    public async Task<Option<User>> GetUserAsync(string email, CancellationToken cancellationToken)
    {
        var filter = new UserFilterBuilder()
            .WithEmail(email)
            .Build();

        return await DatabaseContext.GetOneAsync(Collection, filter, cancellationToken);
    }
    
    public async Task<Option<User>> GetUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var filter = new UserFilterBuilder()
            .WithId(userId)
            .Build();

        return await DatabaseContext.GetOneAsync(Collection, filter, cancellationToken);
    }
}