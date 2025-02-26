using CM25Server.Domain.Commands.Extended;
using Riok.Mapperly.Abstractions;

namespace CM25Server.Domain.Commands.Mappers;

[Mapper]
public partial class UpdateIssueCommandMapper
{
    public partial UpdateIssueExtendedCommand ToExtendedCommand(UpdateIssueCommand command, Guid issueId, Guid userId);
}