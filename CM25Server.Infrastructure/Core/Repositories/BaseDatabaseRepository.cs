using Microsoft.Extensions.Logging;

namespace CM25Server.Infrastructure.Core.Repositories;

public abstract class BaseDatabaseRepository(DatabaseContext databaseContext, string collection, ILogger logger)
{
    protected DatabaseContext DatabaseContext { get; private init; } = databaseContext;
    protected string Collection { get; private init; } = collection;
    protected ILogger Logger { get; private init; } = logger;
}