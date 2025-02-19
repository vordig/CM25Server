using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace CM25Server.Infrastructure.Core;

public static class DatabaseConfiguration
{
    public static IMongoDatabase GetDatabase(string? connectionString)
    {
        var url = new MongoUrl(connectionString);
        var client = new MongoClient(url);
        return client.GetDatabase(url.DatabaseName);
    }
    
    public static void Configure()
    {
        BsonSerializer.RegisterIdGenerator(typeof(Guid), CombGuidGenerator.Instance);
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.CSharpLegacy));

        var pack = new ConventionPack
        {
            new EnumRepresentationConvention(BsonType.String)
        };

        ConventionRegistry.Register("EnumStringConvention", pack, t => true);
    }
}