namespace CM25Server.Domain.Core;

public interface IAuditable
{
    public Audit Audit { get; }
}