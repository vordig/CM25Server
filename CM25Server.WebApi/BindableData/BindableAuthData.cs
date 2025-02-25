using System.Reflection;
using System.Security.Authentication;

namespace CM25Server.WebApi.BindableData;

public record BindableAuthData(Guid UserId)
{
    public static ValueTask<BindableAuthData?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        if (!Guid.TryParse(
                context.User.Identities.First().Claims.First(x => x.Type == "userId").Value, out var userId))
            throw new AuthenticationException("Can not verify a session");

        return ValueTask.FromResult<BindableAuthData?>(
            new BindableAuthData(userId)
        );
    }
}