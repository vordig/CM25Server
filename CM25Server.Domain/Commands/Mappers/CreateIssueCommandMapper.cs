using CM25Server.Domain.Commands.Extended;
using CM25Server.Domain.Enums;
using Riok.Mapperly.Abstractions;

namespace CM25Server.Domain.Commands.Mappers;

[Mapper]
public partial class CreateIssueCommandMapper
{
    [MapValue(nameof(CreateIssueExtendedCommand.State), IssueState.Open)]
    public partial CreateIssueExtendedCommand ToExtendedCommand(CreateIssueCommand command, string code, Guid projectId,
        Guid userId);
}