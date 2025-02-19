using MongoDB.Driver;

namespace CM25Server.Infrastructure.Core.Builders.Sort.Interfaces;

public interface IBaseSortBuilder<T>
{
    public SortDefinitionBuilder<T> Builder { get; }
    public void AddSort(SortDefinition<T> sort);
}