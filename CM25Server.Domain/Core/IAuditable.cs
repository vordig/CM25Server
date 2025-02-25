namespace CM25Server.Domain.Core;

public interface IAuditable : IIdentified
{
    public Audit Audit { get; }
}