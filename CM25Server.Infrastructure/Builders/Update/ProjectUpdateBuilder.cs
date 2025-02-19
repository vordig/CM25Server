using CM25Server.Domain.Commands;
using CM25Server.Domain.Models;
using CM25Server.Infrastructure.Core.Builders.Update;
using CM25Server.Infrastructure.Core.Builders.Update.Interfaces;

namespace CM25Server.Infrastructure.Builders.Update;

public class ProjectUpdateBuilder : BaseUpdateBuilder<Project, ProjectUpdateBuilder>,
    IAuditUpdateBuilder<Project, ProjectUpdateBuilder>
{
    public ProjectUpdateBuilder FromCommand(UpdateProjectCommand command)
    {
        if (command.Name is not null)
        {
            var update = Builder.Set(x => x.Name, command.Name);
            AddUpdate(update);
        }
        
        if (command.Code is not null)
        {
            var update = Builder.Set(x => x.Code, command.Code);
            AddUpdate(update);
        }
        
        if (command.Description is not null)
        {
            var update = Builder.Set(x => x.Description, command.Description);
            AddUpdate(update);
        }
        
        return this;
    }
}