using CM25Server.Domain.Commands.Extended;
using CM25Server.Domain.Models;
using Riok.Mapperly.Abstractions;

namespace CM25Server.Domain.Commands.Mappers;

[Mapper]
public partial class CreateIssueExtendedCommandMapper
{
    public partial Issue ToIssue(CreateIssueExtendedCommand command);
}