using System.Linq.Expressions;

namespace CM25Server.Infrastructure.Builders.Filter.Interfaces;

public interface ISearchFilterBuilder<T> : IBaseFilterBuilder<T>
{
    IReadOnlyCollection<Expression<Func<T, object>>> SearchFields { get; }
}