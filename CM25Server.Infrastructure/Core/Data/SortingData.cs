namespace CM25Server.Infrastructure.Core.Data;

public record SortingData<T>(T SortBy, SortDirection SortDirection) where T : struct, Enum;