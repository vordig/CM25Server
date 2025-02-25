using CM25Server.Domain.Commands.Extended;
using CM25Server.Domain.Commands.Mappers;
using CM25Server.Domain.Commands.Validators;
using CM25Server.Domain.Core;

namespace CM25Server.Domain.Commands;

public record CreateProjectCommand : BaseCommand<CreateProjectCommand, CreateProjectCommandValidator>
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public string Description { get; init; } = string.Empty;

    public CreateProjectExtendedCommand Extend(Guid userId)
    {
        var mapper = new CreateProjectCommandMapper();
        return mapper.ToExtendedCommand(this, userId);
    }
}