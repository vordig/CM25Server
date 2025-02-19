using System.Linq.Expressions;

namespace CM25Server.Infrastructure.Core.Builders.Filter.Interfaces;

public interface ISearchFilterBuilder<T, TBuilder> : IBaseFilterBuilder<T, TBuilder>
{
    public IReadOnlyCollection<Expression<Func<T, object>>> SearchFields { get; }
}