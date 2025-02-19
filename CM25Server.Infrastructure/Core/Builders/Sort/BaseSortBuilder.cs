using CM25Server.Infrastructure.Core.Builders.Sort.Interfaces;
using MongoDB.Driver;

namespace CM25Server.Infrastructure.Core.Builders.Sort;

public abstract class BaseSortBuilder<T, TSortBuilder> : IBaseSortBuilder<T>
    where TSortBuilder : BaseSortBuilder<T, TSortBuilder>
{
    public SortDefinitionBuilder<T> Builder => Builders<T>.Sort;

    protected SortDefinition<T>? Sort;

    public void AddSort(SortDefinition<T> sort)
    {
        Sort = Sort is null ? sort : Builder.Combine(Sort, sort);
    }

    public SortDefinition<T> Build() => Sort ?? DefaultSort;

    protected abstract SortDefinition<T> DefaultSort { get; }
}