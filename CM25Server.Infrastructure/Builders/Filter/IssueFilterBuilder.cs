using System.Linq.Expressions;
using CM25Server.Domain.Enums;
using CM25Server.Domain.Models;
using CM25Server.Infrastructure.Core.Builders.Filter;
using CM25Server.Infrastructure.Core.Builders.Filter.Interfaces;

namespace CM25Server.Infrastructure.Builders.Filter;

public class IssueFilterBuilder : BaseFilterBuilder<Issue, IssueFilterBuilder>,
    IIdFilterBuilder<Issue, IssueFilterBuilder>, ISearchFilterBuilder<Issue, IssueFilterBuilder>,
    IOwnedByUserFilterBuilder<Issue, IssueFilterBuilder>
{
    public IReadOnlyCollection<Expression<Func<Issue, object>>> SearchFields =>
    [
        x => x.Code,
        x => x.Name,
        x => x.Description
    ];
    
    public IssueFilterBuilder ForProject(Guid projectId)
    {
        var filter = Builder.Eq(x => x.ProjectId, projectId);
        AddFilter(filter);
        return this;
    }
    
    public IssueFilterBuilder WithState(IssueState? state)
    {
        if (state is null) return this;
        
        var filter = Builder.Eq(x => x.State, state);
        AddFilter(filter);
        return this;
    }
    
    public IssueFilterBuilder WithPriorities(IReadOnlyCollection<IssuePriority>? priorities)
    {
        if (priorities is null) return this;
        
        var filter = Builder.In(x => x.Priority, priorities);
        AddFilter(filter);
        return this;
    }
}