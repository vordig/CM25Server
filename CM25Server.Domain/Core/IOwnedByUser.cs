namespace CM25Server.Domain.Core;

public interface IOwnedByUser : IAuditable
{
    public Guid UserId { get; }
}