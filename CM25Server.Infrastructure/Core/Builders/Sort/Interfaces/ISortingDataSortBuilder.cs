using System.Linq.Expressions;

namespace CM25Server.Infrastructure.Core.Builders.Sort.Interfaces;

public interface ISortingDataSortBuilder<T, TSortBy, TBuilder> : IBaseSortBuilder<T> where TSortBy : struct, Enum
{
    public Expression<Func<T, object>> GetSortFieldExpression(TSortBy sortBy);
}