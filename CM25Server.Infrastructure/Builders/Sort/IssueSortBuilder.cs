using System.Linq.Expressions;
using CM25Server.Domain.Models;
using CM25Server.Infrastructure.Core.Builders.Sort;
using CM25Server.Infrastructure.Core.Builders.Sort.Interfaces;
using CM25Server.Infrastructure.Enums;
using MongoDB.Driver;

namespace CM25Server.Infrastructure.Builders.Sort;

public class IssueSortBuilder : BaseSortBuilder<Issue, IssueSortBuilder>,
    ISortingDataSortBuilder<Issue, IssueSortBy, IssueSortBuilder>
{
    protected override SortDefinition<Issue> DefaultSort => Builder.Ascending(x => x.Audit.ModifiedOn);
    
    public Expression<Func<Issue, object>> GetSortFieldExpression(IssueSortBy sortBy)
    {
        return sortBy switch
        {
            IssueSortBy.Name => x => x.Name,
            IssueSortBy.Code => x => x.Code,
            IssueSortBy.Priority => x => x.Priority,
            IssueSortBy.CreatedOn => x => x.Audit.CreatedOn,
            IssueSortBy.ModifiedOn => x => x.Audit.ModifiedOn,
            _ => throw new ArgumentOutOfRangeException(nameof(sortBy), sortBy, null)
        };
    }
}