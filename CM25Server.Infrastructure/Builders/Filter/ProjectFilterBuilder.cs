using System.Linq.Expressions;
using CM25Server.Domain.Models;
using CM25Server.Infrastructure.Core.Builders.Filter;
using CM25Server.Infrastructure.Core.Builders.Filter.Interfaces;

namespace CM25Server.Infrastructure.Builders.Filter;

public class ProjectFilterBuilder : BaseFilterBuilder<Project, ProjectFilterBuilder>, IIdFilterBuilder<Project>,
    ISearchFilterBuilder<Project>
{
    public IReadOnlyCollection<Expression<Func<Project, object>>> SearchFields =>
    [
        x => x.Code,
        x => x.Name,
        x => x.Description
    ];
}