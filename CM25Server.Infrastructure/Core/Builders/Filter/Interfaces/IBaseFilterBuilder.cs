using MongoDB.Driver;

namespace CM25Server.Infrastructure.Core.Builders.Filter.Interfaces;

public interface IBaseFilterBuilder<T, TBuilder>
{
    public FilterDefinitionBuilder<T> Builder { get; }
    public void AddFilter(FilterDefinition<T> filter);
}