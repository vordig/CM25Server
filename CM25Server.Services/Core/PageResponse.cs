namespace CM25Server.Services.Core;

public record PageResponse<TResponse>
{
    public required long Total { get; init; }
    public required IReadOnlyCollection<TResponse> Items { get; init; }
    public required int PageNumber { get; init; }
    public required int PageSize { get; init; }

    public long TotalPages
    {
        get
        {
            var ps = PageSize > 0 ? PageSize : 1;
            return (Total + ps - 1) / ps;
        }
    }
}