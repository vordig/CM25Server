using System.Reflection;
using CM25Server.Infrastructure.Core.Data;

namespace CM25Server.WebApi.BindableData;

public record BindablePagingData(int PageNumber, int PageSize) : PagingData(PageNumber, PageSize)
{
    public static ValueTask<BindablePagingData?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        const string pageNumberKey = "pageNumber";
        const string pageSizeKey = "pageSize";

        var pageNumber = 1;
        var pageSource = context.Request.Query[pageNumberKey].FirstOrDefault();
        if (pageSource is not null)
            _ = int.TryParse(pageSource, out pageNumber);

        var pageSize = 25;
        var pageSizeSource = context.Request.Query[pageSizeKey].FirstOrDefault();
        if (pageSizeSource is not null)
            _ = int.TryParse(pageSizeSource, out pageSize);

        return ValueTask.FromResult<BindablePagingData?>(new BindablePagingData(pageNumber, pageSize));
    }
}