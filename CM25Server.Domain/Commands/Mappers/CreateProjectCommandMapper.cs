using CM25Server.Domain.Commands.Extended;
using Riok.Mapperly.Abstractions;

namespace CM25Server.Domain.Commands.Mappers;

[Mapper]
public partial class CreateProjectCommandMapper
{
    public partial CreateProjectExtendedCommand ToExtendedCommand(CreateProjectCommand command, Guid userId);
}