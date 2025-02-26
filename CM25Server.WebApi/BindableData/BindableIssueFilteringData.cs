using System.Reflection;
using CM25Server.Domain.Enums;
using CM25Server.Infrastructure.Data;

namespace CM25Server.WebApi.BindableData;

public record BindableIssueFilteringData : IssueFilteringData
{
    public static ValueTask<BindableIssueFilteringData?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        const string searchTermKey = "searchTerm";
        const string stateKey = "state";
        const string prioritiesKey = "priority";
        const string projectIdsKey = "projectId";

        var searchTerm = context.Request.Query[searchTermKey];
        IssueState? state = null;
        List<IssuePriority>? priorities = null;
        List<Guid>? projectIds = null;
        
        var stateQuery = context.Request.Query[stateKey];
        if (stateQuery.Count != 0)
        {
            Enum.TryParse<IssueState>(stateQuery, ignoreCase: true, out var issueState);
            state = issueState;
        }
        
        var prioritiesQuery = context.Request.Query[prioritiesKey];
        if (prioritiesQuery.Count != 0)
        {
            priorities = [];
            foreach (var priority in prioritiesQuery)
            {
                Enum.TryParse<IssuePriority>(priority, ignoreCase: true, out var issuePriority);
                priorities.Add(issuePriority);
            }
        }
        
        var projectsQuery = context.Request.Query[projectIdsKey];
        if (projectsQuery.Count != 0)
        {
            projectIds = [];
            foreach (var projectId in projectsQuery)
            {
                if (Guid.TryParse(projectId, out var parsedProjectId))
                    projectIds.Add(parsedProjectId);
            }
        }

        return ValueTask.FromResult<BindableIssueFilteringData?>(
            new BindableIssueFilteringData
            {
                SearchTerm = searchTerm,
                State = state,
                Priorities = priorities,
                ProjectIds = projectIds
            }
        );
    }
}