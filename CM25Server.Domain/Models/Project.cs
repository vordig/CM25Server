using CM25Server.Domain.Commands.Extended;
using CM25Server.Domain.Commands.Mappers;
using CM25Server.Domain.Core;

namespace CM25Server.Domain.Models;

public class Project : BaseOwnedByUserModel
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;

    public static Project FromCommand(CreateProjectExtendedCommand command)
    {
        var mapper = new CreateProjectExtendedCommandMapper();
        return mapper.ToProject(command);
    }
}