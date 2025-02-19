using System.Collections.Immutable;
using Bogus;
using CM25Server.Domain.Enums;
using CM25Server.Domain.Models;
using CM25Server.Services.Contracts;
using CM25Server.Services.Contracts.Requests;
using LanguageExt;
using LanguageExt.Common;

namespace CM25Server.Services;

public class IssueService(ProjectService projectService)
{
    private readonly List<Issue> _issues = [];

    private readonly Faker<Issue> _issueGenerator = new Faker<Issue>("ru")
        .RuleFor(p => p.Name, f => f.Commerce.Product())
        .RuleFor(p => p.Description, (f, i) => f.Rant.Review(i.Name))
        .RuleFor(p => p.Priority, f => f.PickRandom<IssuePriority>());

    public Result<IImmutableList<Issue>> GetIssues(Guid projectId) =>
        _issues.Where(x => x.ProjectId == projectId).ToImmutableList();

    public Option<Issue> GetIssue(Guid id, Guid projectId) =>
        _issues.SingleOrDefault(x => x.Id == id && x.ProjectId == projectId);

    public Result<Issue> CreateIssue(IssueRequest request, Guid projectId)
    {
        throw new NotImplementedException();
        
        // var project = projectService.GetProjectAsync(projectId);
        //
        // return project.Match(
        //     existingProject =>
        //     {
        //         var issue = new Issue
        //         {
        //             ProjectId = existingProject.Id,
        //             Code = $"{existingProject.Code}-{existingProject.IssueCounter++}",
        //             Name = request.Name,
        //             Description = request.Description,
        //             Priority = request.Priority,
        //             State = IssueState.Unresolved,
        //         };
        //         _issues.Add(issue);
        //         return issue;
        //     },
        //     new Result<Issue>(new Exception($"Project {projectId} does not exist."))
        // );
    }

    public Result<Issue> UpdateIssue(Guid id, IssueRequest request, Guid projectId)
    {
        var issue = GetIssue(id, projectId);

        return issue.Match(
            existingIssue =>
            {
                existingIssue.Name = request.Name;
                existingIssue.Description = request.Description;
                existingIssue.Priority = request.Priority;
                existingIssue.State = request.State;
                existingIssue.Audit.ModifiedOn = DateTime.Now;
                return existingIssue;
            },
            new Result<Issue>(new Exception($"Issue {id} for Project {projectId} does not exist."))
        );
    }

    public Result<bool> DeleteIssue(Guid id, Guid projectId)
    {
        var issue = GetIssue(id, projectId);

        return issue.Match(
            _issues.Remove,
            false
        );
    }

    public Result<int> GenerateIssues(int count, Guid projectId)
    {
        throw new NotImplementedException();
        
        // var project = projectService.GetProjectAsync(projectId);
        //
        // return project.Match(
        //     existingProject =>
        //     {
        //         var issues = _issueGenerator.Generate(count);
        //
        //         foreach (var issue in issues)
        //         {
        //             issue.ProjectId = existingProject.Id;
        //             issue.Code = $"{existingProject.Code}-{existingProject.IssueCounter++}";
        //         }
        //
        //         _issues.AddRange(issues);
        //         return count;
        //     },
        //     new Result<int>(new Exception($"Project {projectId} does not exist."))
        // );
    }

    public Result<int> ClearIssues(Guid projectId) => _issues.RemoveAll(x => x.ProjectId == projectId);
}