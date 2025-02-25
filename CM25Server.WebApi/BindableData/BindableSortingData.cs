using System.Reflection;
using CM25Server.Infrastructure.Core.Data;

namespace CM25Server.WebApi.BindableData;

public record BindableSortingData<TEnum>(TEnum SortBy, SortDirection SortDirection) :
    SortingData<TEnum>(SortBy, SortDirection) where TEnum : struct, Enum
{
    public static ValueTask<BindableSortingData<TEnum>?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        const string sortByKey = "sortBy";
        const string sortDirectionKey = "sortDir";

        Enum.TryParse<TEnum>(context.Request.Query[sortByKey], ignoreCase: true, out var sortBy);
        Enum.TryParse<SortDirection>(context.Request.Query[sortDirectionKey], ignoreCase: true, out var sortDirection);

        return ValueTask.FromResult<BindableSortingData<TEnum>?>(
            new BindableSortingData<TEnum>(sortBy, sortDirection)
        );
    }
}