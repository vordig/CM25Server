using System.Linq.Expressions;

namespace CM25Server.Infrastructure.Core.Builders.Filter.Interfaces;

public interface ISearchFilterBuilder<T> : IBaseFilterBuilder<T>
{
    public IReadOnlyCollection<Expression<Func<T, object>>> SearchFields { get; }
}