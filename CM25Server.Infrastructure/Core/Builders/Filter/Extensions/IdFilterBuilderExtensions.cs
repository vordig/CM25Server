using CM25Server.Domain.Core;
using CM25Server.Infrastructure.Core.Builders.Filter.Interfaces;

namespace CM25Server.Infrastructure.Core.Builders.Filter.Extensions;

public static class IdFilterBuilderExtensions
{
    public static TBuilder WithId<T, TBuilder>(this IIdFilterBuilder<T, TBuilder> builder, Guid id)
        where T : IIdentified
    {
        var filter = builder.Builder.Eq(x => x.Id, id);
        builder.AddFilter(filter);
        return (TBuilder)builder;
    }
}