namespace CM25Server.Domain.Core;

public abstract class BaseModel : IIdentified, IAuditable
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Audit Audit { get; init; } = new();
}