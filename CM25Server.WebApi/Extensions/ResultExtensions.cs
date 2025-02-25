using CM25Server.Domain.Exceptions;
using LanguageExt;
using LanguageExt.Common;

namespace CM25Server.WebApi.Extensions;

public static class ResultExtensions
{
    public static IResult ToOkResponse<T>(this Result<T> result)
    {
        return result.Match(
            Results.Ok,
            exception =>
            {
                if (exception is ProblemException problemException)
                {
                    return Results.Problem(
                        type: "Bad Request",
                        title: problemException.Error,
                        detail: problemException.Message,
                        statusCode: StatusCodes.Status400BadRequest
                    );
                }
                
                return Results.Problem(
                    detail: exception.Message
                );
            });
    }
    
    public static IResult ToOkResponse<T>(this Option<T> result) => result.Match(Results.Ok, Results.NotFound());
}