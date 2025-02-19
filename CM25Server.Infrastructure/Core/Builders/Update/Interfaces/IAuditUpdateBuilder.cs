using CM25Server.Domain.Core;

namespace CM25Server.Infrastructure.Core.Builders.Update.Interfaces;

public interface IAuditUpdateBuilder<T, TBuilder> : IBaseUpdateBuilder<T, TBuilder> where T : IAuditable
    where TBuilder : IBaseUpdateBuilder<T, TBuilder>
{
}