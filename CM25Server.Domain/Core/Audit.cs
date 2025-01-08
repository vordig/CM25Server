namespace CM25Server.Domain.Core;

public record Audit
{
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public DateTime ModifiedOn { get; set; } = DateTime.Now;
}