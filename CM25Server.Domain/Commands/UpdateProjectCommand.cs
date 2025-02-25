using CM25Server.Domain.Commands.Extended;
using CM25Server.Domain.Commands.Mappers;
using CM25Server.Domain.Commands.Validators;
using CM25Server.Domain.Core;

namespace CM25Server.Domain.Commands;

public record UpdateProjectCommand : BaseCommand<UpdateProjectCommand, UpdateProjectCommandValidator>
{
    public string? Code { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    
    public UpdateProjectExtendedCommand Extend(Guid projectId, Guid userId)
    {
        var mapper = new UpdateProjectCommandMapper();
        return mapper.ToExtendedCommand(this, projectId, userId);
    }
}