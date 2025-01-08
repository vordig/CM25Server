using CM25Server.Domain.Core;

namespace CM25Server.Domain.Models;

public class Project : BaseModel
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public int IssueCounter { get; private init; } = 0;
}