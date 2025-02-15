using CM25Server.Infrastructure.Builders.Filter.Interfaces;
using MongoDB.Driver;

namespace CM25Server.Infrastructure.Builders.Filter;

public abstract class BaseFilterBuilder<T, TFilterBuilder> : IBaseFilterBuilder<T>
    where TFilterBuilder : BaseFilterBuilder<T, TFilterBuilder>
{
    public FilterDefinitionBuilder<T> Builder => Builders<T>.Filter;

    protected FilterDefinition<T> Filter = Builders<T>.Filter.Empty;

    public void AddFilter(FilterDefinition<T> filter)
    {
        Filter &= filter;
    }

    public FilterDefinition<T> Build() => Filter;
}