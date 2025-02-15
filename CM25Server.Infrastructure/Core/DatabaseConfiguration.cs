using MongoDB.Driver;

namespace CM25Server.Infrastructure.Core;

public static class DatabaseConfiguration
{
    public static IMongoDatabase GetDatabase(string? connectionString, string? databaseName)
    {
        var url = new MongoUrl(connectionString);
        var client = new MongoClient(url);
        return client.GetDatabase(databaseName);
    }
}