using CM25Server.Infrastructure.Core.Builders.Sort.Interfaces;
using CM25Server.Infrastructure.Core.Data;

namespace CM25Server.Infrastructure.Core.Builders.Sort.Extensions;

public static class SortingDataSortBuilderExtensions
{
    public static TBuilder SortBy<T, TSortBy, TBuilder>(this ISortingDataSortBuilder<T, TSortBy, TBuilder> builder,
        SortingData<TSortBy> sortingData) where TSortBy : struct, Enum
    {
        var sortFieldExpression = builder.GetSortFieldExpression(sortingData.SortBy);
        var sort = sortingData.SortDirection == SortDirection.Asc
            ? builder.Builder.Ascending(sortFieldExpression)
            : builder.Builder.Descending(sortFieldExpression);

        builder.AddSort(sort);
        return (TBuilder)builder;
    }
}