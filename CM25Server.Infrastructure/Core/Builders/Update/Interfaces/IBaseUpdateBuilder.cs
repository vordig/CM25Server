using MongoDB.Driver;

namespace CM25Server.Infrastructure.Core.Builders.Update.Interfaces;

public interface IBaseUpdateBuilder<T, TBuilder>
{
    public UpdateDefinitionBuilder<T> Builder { get; }
    public void AddUpdate(UpdateDefinition<T> update);
}