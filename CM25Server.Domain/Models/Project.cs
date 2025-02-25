using CM25Server.Domain.Commands;
using CM25Server.Domain.Core;

namespace CM25Server.Domain.Models;

public class Project : BaseModel, IOwnedByUser
{
    public required Guid UserId { get; init; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;

    public static Project FromCommand(CreateProjectCommand command, Guid userId)
    {
        var result = new Project
        {
            UserId = userId,
            Name = command.Name,
            Description = command.Description,
            Code = command.Code
        };
        return result;
    }
}