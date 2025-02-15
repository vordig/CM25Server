using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CM25Server.Infrastructure.Core;

public class DatabaseContext(IMongoDatabase database, ILogger<DatabaseContext> logger)
{
    public IMongoCollection<T> Collection<T>(string collectionName) => database.GetCollection<T>(collectionName);

    public async Task<Result<long>> CreateOneAsync<T>(string collectionName, T entity,
        CancellationToken cancellationToken)
    {
        try
        {
            await Collection<T>(collectionName)
                .InsertOneAsync(entity, null, cancellationToken);
            return 1;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to create entity");
            return new Result<long>(exception);
        }
    }

    public async Task<Result<long>> CreateManyAsync<T>(string collectionName, IReadOnlyCollection<T> entities,
        CancellationToken cancellationToken)
    {
        try
        {
            await Collection<T>(collectionName)
                .InsertManyAsync(entities, null, cancellationToken);
            return entities.Count;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to create entities");
            return new Result<long>(exception);
        }
    }

    public async Task<Result<long>> UpdateOneAsync<T>(string collectionName, FilterDefinition<T> filter,
        UpdateDefinition<T> update, CancellationToken cancellationToken)
    {
        try
        {
            var updateResult = await Collection<T>(collectionName)
                .UpdateOneAsync(filter, update, null, cancellationToken);
            return updateResult.ModifiedCount;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to update entity");
            return new Result<long>(exception);
        }
    }

    public async Task<Result<long>> UpdateManyAsync<T>(string collectionName, FilterDefinition<T> filter,
        UpdateDefinition<T> update, CancellationToken cancellationToken)
    {
        try
        {
            var updateResult = await Collection<T>(collectionName)
                .UpdateManyAsync(filter, update, null, cancellationToken);
            return updateResult.ModifiedCount;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to update entities");
            return new Result<long>(exception);
        }
    }

    public async Task<Result<long>> DeleteOneAsync<T>(string collectionName, FilterDefinition<T> filter,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await Collection<T>(collectionName)
                .DeleteOneAsync(filter, cancellationToken);
            return result.DeletedCount;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to delete entity");
            return new Result<long>(exception);
        }
    }

    public async Task<Result<long>> DeleteManyAsync<T>(string collectionName, FilterDefinition<T> filter,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await Collection<T>(collectionName)
                .DeleteManyAsync(filter, cancellationToken);
            return result.DeletedCount;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to delete entities");
            return new Result<long>(exception);
        }
    }

    public async Task<Result<long>> CountAsync<T>(string collectionName, FilterDefinition<T> filter,
        CancellationToken cancellationToken)
    {
        try
        {
            return await Collection<T>(collectionName)
                .CountDocumentsAsync(filter, null, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to count entities");
            return new Result<long>(exception);
        }
    }

    public async Task<Result<bool>> AnyAsync<T>(string collectionName, FilterDefinition<T> filter,
        CancellationToken cancellationToken)
    {
        try
        {
            return await Collection<T>(collectionName)
                .Find(filter)
                .AnyAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to find any entities");
            return new Result<bool>(exception);
        }
    }

    public async Task<Option<T>> GetOneAsync<T>(string collectionName, FilterDefinition<T> filter,
        CancellationToken cancellationToken)
    {
        return await Collection<T>(collectionName)
            .Find(filter)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Option<IReadOnlyCollection<T>>> GetManyAsync<T>(string collectionName, FilterDefinition<T> filter,
        SortDefinition<T> sort, CancellationToken cancellationToken)
    {
        var options = new FindOptions
        {
            Collation = new Collation("en", caseLevel: false, numericOrdering: true)
        };
        return await Collection<T>(collectionName)
            .Find(filter, options)
            .Sort(sort)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<IReadOnlyCollection<T>>> GetPageAsync<T>(string collectionName, FilterDefinition<T> filter,
        SortDefinition<T> sort, int page, int pageSize, CancellationToken cancellationToken)
    {
        var options = new FindOptions
        {
            Collation = new Collation("en", caseLevel: false, numericOrdering: true)
        };
        return await Collection<T>(collectionName)
            .Find(filter, options)
            .Sort(sort)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
    }
}