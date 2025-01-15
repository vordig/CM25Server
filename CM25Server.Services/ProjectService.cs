using System.Collections.Immutable;
using Bogus;
using CM25Server.Domain.Models;
using CM25Server.Services.Contracts;
using LanguageExt;
using LanguageExt.Common;

namespace CM25Server.Services;

public class ProjectService
{
    private readonly List<Project> _projects = [];

    private readonly Faker<Project> _projectGenerator = new Faker<Project>("ru")
        .RuleFor(p => p.Name, f => f.Company.CompanyName())
        .RuleFor(p => p.Description, f => f.Company.Bs())
        .RuleFor(p => p.Code, (f, p) => p.Name[..2].ToUpper());
    
    public Result<IImmutableList<Project>> GetProjects() => _projects.ToImmutableList();
    public Option<Project> GetProject(Guid id) => _projects.SingleOrDefault(x => x.Id == id);
    
    public Result<Project> CreateProject(ProjectRequest request)
    {
        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            Code = request.Code
        };
        _projects.Add(project);
        return project;
    }
    
    public Result<Project> UpdateProject(Guid id, ProjectRequest request)
    {
        var project = GetProject(id);

        return project.Match(
            existingProject =>
            {
                existingProject.Name = request.Name;
                existingProject.Description = request.Description;
                existingProject.Code = request.Code;
                existingProject.Audit.ModifiedOn = DateTime.Now;
                return existingProject;
            },
            new Result<Project>(new Exception($"Project {id} does not exist."))
        );
    }

    public Result<bool> DeleteProject(Guid id)
    {
        var project = GetProject(id);

        return project.Match(
            _projects.Remove,
            false
        );
    }
    
    public Result<int> GenerateProjects(int count)
    {
        var projects = _projectGenerator.Generate(count);
        _projects.AddRange(projects);
        return count;
    }
    
    public Result<int> ClearProjects() => _projects.RemoveAll(x => true);
}