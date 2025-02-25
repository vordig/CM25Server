using CM25Server.Domain.Commands.Extended;
using Riok.Mapperly.Abstractions;

namespace CM25Server.Domain.Commands.Mappers;

[Mapper]
public partial class UpdateProjectCommandMapper
{
    public partial UpdateProjectExtendedCommand ToExtendedCommand(UpdateProjectCommand command, Guid projectId,
        Guid userId);
}