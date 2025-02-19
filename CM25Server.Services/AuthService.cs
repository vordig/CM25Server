using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using CM25Server.Domain.Commands;
using CM25Server.Infrastructure.Core.Options;
using CM25Server.Services.Core;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CM25Server.Services;

public class AuthService(IOptions<AuthOptions> authOptions, ILogger<AuthService> logger)
{
    private readonly AuthOptions _authOptions = authOptions.Value;

    public async Task<Result<AuthResponse>> LoginAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        if (command.Password != "password")
            return new Result<AuthResponse>(new AuthenticationException("Invalid password"));

        return new AuthResponse
        {
            AccessToken = EncodeAccessToken(command.Email)
        };
    }

    private string EncodeAccessToken(string email)
    {
        var identity = new ClaimsIdentity([
            new Claim("id", Guid.NewGuid().ToString()),
            new Claim("name", "Test user"),
            new Claim("email", "test@email.com")
        ]);

        var secret = Encoding.UTF8.GetBytes(_authOptions.JWTSecret!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = identity,
            Issuer = _authOptions.JWTIssuer,
            Audience = _authOptions.JWTAudience,
            Expires = DateTime.UtcNow.AddMinutes(_authOptions.JWTTimeToLive),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secret),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(securityToken);
    }
}