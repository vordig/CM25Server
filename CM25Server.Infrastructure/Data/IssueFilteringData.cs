using CM25Server.Domain.Enums;

namespace CM25Server.Infrastructure.Data;

public record IssueFilteringData
{
    public string? SearchTerm { get; init; }
    public IssueState? State { get; init; }
    public IReadOnlyCollection<IssuePriority>? Priorities { get; init; }
    public IReadOnlyCollection<Guid>? ProjectIds { get; init; }
}