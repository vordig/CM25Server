using System.Linq.Expressions;
using CM25Server.Domain.Models;
using CM25Server.Infrastructure.Core.Builders.Filter;
using CM25Server.Infrastructure.Core.Builders.Filter.Interfaces;

namespace CM25Server.Infrastructure.Builders.Filter;

public class IssueFilterBuilder : BaseFilterBuilder<Issue, IssueFilterBuilder>, IIdFilterBuilder<Issue>,
    ISearchFilterBuilder<Issue>
{
    public IReadOnlyCollection<Expression<Func<Issue, object>>> SearchFields =>
    [
        x => x.Code,
        x => x.Name,
        x => x.Description
    ];
}