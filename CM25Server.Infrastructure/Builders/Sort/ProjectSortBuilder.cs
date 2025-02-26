using System.Linq.Expressions;
using CM25Server.Domain.Models;
using CM25Server.Infrastructure.Core.Builders.Sort;
using CM25Server.Infrastructure.Core.Builders.Sort.Interfaces;
using CM25Server.Infrastructure.Enums;
using MongoDB.Driver;

namespace CM25Server.Infrastructure.Builders.Sort;

public class ProjectSortBuilder : BaseSortBuilder<Project, ProjectSortBuilder>,
    ISortingDataSortBuilder<Project, ProjectSortBy, ProjectSortBuilder>
{
    protected override SortDefinition<Project> DefaultSort => Builder.Ascending(x => x.Name);
    
    public Expression<Func<Project, object>> GetSortFieldExpression(ProjectSortBy sortBy)
    {
        return sortBy switch
        {
            ProjectSortBy.Name => x => x.Name,
            ProjectSortBy.Code => x => x.Code,
            ProjectSortBy.CreatedOn => x => x.Audit.CreatedOn,
            ProjectSortBy.ModifiedOn => x => x.Audit.ModifiedOn,
            _ => throw new ArgumentOutOfRangeException(nameof(sortBy), sortBy, null)
        };
    }
}