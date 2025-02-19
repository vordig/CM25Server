using System.Reflection;
using CM25Server.Infrastructure.Data;

namespace CM25Server.WebApi.Data;

public record BindableProjectFilteringData(string? SearchTerm) : ProjectFilteringData(SearchTerm)
{
    public static ValueTask<BindableProjectFilteringData?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        const string searchTermKey = "searchTerm";

        var searchTerm = context.Request.Query[searchTermKey];

        return ValueTask.FromResult<BindableProjectFilteringData?>(
            new BindableProjectFilteringData(searchTerm)
        );
    }
}