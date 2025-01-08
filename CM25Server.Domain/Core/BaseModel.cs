namespace CM25Server.Domain.Core;

public abstract class BaseModel : IIdentified
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Audit Audit { get; init; } = new();
}