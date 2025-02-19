using CM25Server.Domain.Core;
using CM25Server.Infrastructure.Core.Builders.Update.Interfaces;
using MongoDB.Driver;

namespace CM25Server.Infrastructure.Core.Builders.Update.Extensions;

public static class AuditUpdateBuilder
{
    public static TBuilder UpdateModifiedOn<T, TBuilder>(this IAuditUpdateBuilder<T, TBuilder> builder)
        where T : IAuditable where TBuilder : IBaseUpdateBuilder<T, TBuilder>
    {
        var update = builder.Builder
            .CurrentDate(x => x.Audit.ModifiedOn, UpdateDefinitionCurrentDateType.Date);
        
        builder.AddUpdate(update);
        return (TBuilder)builder;
    }
}