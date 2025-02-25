using CM25Server.Domain.Commands;
using CM25Server.Domain.Models;
using CM25Server.Infrastructure.Core.Builders.Update;
using CM25Server.Infrastructure.Core.Builders.Update.Interfaces;

namespace CM25Server.Infrastructure.Builders.Update;

public class IssueUpdateBuilder : BaseUpdateBuilder<Issue, IssueUpdateBuilder>,
    IAuditUpdateBuilder<Issue, IssueUpdateBuilder>
{
    public IssueUpdateBuilder FromCommand(UpdateIssueCommand command)
    {
        if (command.Name is not null)
        {
            var update = Builder.Set(x => x.Name, command.Name);
            AddUpdate(update);
        }
        
        if (command.Description is not null)
        {
            var update = Builder.Set(x => x.Description, command.Description);
            AddUpdate(update);
        }
        
        if (command.Priority is not null)
        {
            var update = Builder.Set(x => x.Priority, command.Priority);
            AddUpdate(update);
        }
        
        if (command.State is not null)
        {
            var update = Builder.Set(x => x.State, command.State);
            AddUpdate(update);
        }
        
        return this;
    }
}