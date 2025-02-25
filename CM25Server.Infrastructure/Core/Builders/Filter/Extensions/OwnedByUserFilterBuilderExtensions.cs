using CM25Server.Domain.Core;
using CM25Server.Infrastructure.Core.Builders.Filter.Interfaces;

namespace CM25Server.Infrastructure.Core.Builders.Filter.Extensions;

public static class OwnedByUserFilterBuilderExtensions
{
    public static TBuilder OwnedByUser<T, TBuilder>(this IOwnedByUserFilterBuilder<T, TBuilder> builder, Guid userId)
        where T : IOwnedByUser
    {
        var filter = builder.Builder.Eq(x => x.UserId, userId);
        builder.AddFilter(filter);
        return (TBuilder)builder;
    }
}