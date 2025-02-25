namespace CM25Server.Domain.Core;

public abstract class BaseOwnedByUserModel : BaseModel, IOwnedByUser
{
    public required Guid UserId { get; init; }
}