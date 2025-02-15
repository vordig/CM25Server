using CM25Server.Infrastructure.Builders.Filter.Interfaces;
using MongoDB.Bson;

namespace CM25Server.Infrastructure.Builders.Filter.Extensions;

public static class SearchFilterBuilderExtensions
{
    public static TBuilder SearchFor<T, TBuilder>(this ISearchFilterBuilder<T> builder, string? searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
            return (TBuilder)builder;

        var filter = builder.Builder.Or(
            builder.SearchFields.Select(field =>
                builder.Builder.Regex(field, new BsonRegularExpression(searchTerm, "i")))
        );

        builder.AddFilter(filter);
        return (TBuilder)builder;
    }
}