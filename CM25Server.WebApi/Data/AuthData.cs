using System.Reflection;
using System.Security.Authentication;
using Microsoft.IdentityModel.JsonWebTokens;

namespace CM25Server.WebApi.Data;

public record AuthData(Guid UserId)
{
    public static ValueTask<AuthData?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        if (!Guid.TryParse(
                context.User.Identities.First().Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value,
                out var userId))
            throw new AuthenticationException("Can not verify a session");

        return ValueTask.FromResult<AuthData?>(
            new AuthData(userId)
        );
    }
}