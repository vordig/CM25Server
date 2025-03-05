namespace CM25Server.Domain.Core;

public abstract class BaseModel : IAuditable
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Audit Audit { get; init; } = new();
}