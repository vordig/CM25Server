using CM25Server.Infrastructure.Core.Builders.Update.Interfaces;
using MongoDB.Driver;

namespace CM25Server.Infrastructure.Core.Builders.Update;

public class BaseUpdateBuilder<T, TBuilder> : IBaseUpdateBuilder<T, TBuilder>
{
    public UpdateDefinitionBuilder<T> Builder => Builders<T>.Update;

    protected UpdateDefinition<T>? Update;

    public void AddUpdate(UpdateDefinition<T> update)
    {
        Update = Update is null ? update : Builder.Combine(Update, update);
    }

    public UpdateDefinition<T> Build() => Update ?? Builder.Combine();
}