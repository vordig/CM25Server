using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CM25Server.Domain.Commands;
using CM25Server.Domain.Exceptions;
using CM25Server.Domain.Models;
using CM25Server.Infrastructure.Core.Options;
using CM25Server.Infrastructure.Repositories;
using CM25Server.Services.Core;
using LanguageExt.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CM25Server.Services;

public class AuthService(UserRepository userRepository, IOptions<AuthOptions> authOptions, ILogger<AuthService> logger)
{
    private readonly AuthOptions _authOptions = authOptions.Value;

    public async Task<Result<AuthResponse>> SignInAsync(SignInCommand command, CancellationToken cancellationToken)
    {
        var commandValidationResult = await command.ValidateAsync(cancellationToken);
        return await commandValidationResult.Match(
            async validatedCommand => await AuthorizeUserAsync(validatedCommand, cancellationToken),
            exception => Task.FromResult(new Result<AuthResponse>(exception))
        );
    }

    public async Task<Result<AuthResponse>> SignUpAsync(SignUpCommand command, CancellationToken cancellationToken)
    {
        var commandValidationResult = await command.ValidateAsync(cancellationToken);
        return await commandValidationResult.Match(
            async validatedCommand => await CreateUniqueUserAsync(validatedCommand, cancellationToken),
            exception => Task.FromResult(new Result<AuthResponse>(exception))
        );
    }

    public async Task<Result<AuthResponse>> RefreshAsync(RefreshCommand command, CancellationToken cancellationToken)
    {
        var decodeRefreshTokenResult = DecodeRefreshToken(command.Token);
        return await decodeRefreshTokenResult.Match(
            async userId =>
            {
                var existingUserResult = await userRepository.GetUserAsync(userId, cancellationToken);
                return existingUserResult.Match(
                    existingUser => EncodeAuthResponse(existingUser),
                    () => new Result<AuthResponse>(new ProblemException("Authorization refresh failed",
                        "Incorrect user"))
                );
            },
            exception => Task.FromResult(new Result<AuthResponse>(exception))
        );
    }

    private async Task<Result<AuthResponse>> AuthorizeUserAsync(SignInCommand command,
        CancellationToken cancellationToken)
    {
        var existingUserResult = await userRepository.GetUserAsync(command.Email, cancellationToken);
        return existingUserResult.Match(
            existingUser =>
            {
                var passwordHasher = new PasswordHasher<SignInCommand>();
                var verificationResult =
                    passwordHasher.VerifyHashedPassword(command, existingUser.Password, command.Password);

                return verificationResult switch
                {
                    PasswordVerificationResult.Failed => new Result<AuthResponse>(
                        new ProblemException("Authorization failed", "Incorrect credentials")),
                    PasswordVerificationResult.Success => EncodeAuthResponse(existingUser),
                    PasswordVerificationResult.SuccessRehashNeeded => EncodeAuthResponse(existingUser),
                    _ => new Result<AuthResponse>(new ArgumentOutOfRangeException())
                };
            },
            () => new Result<AuthResponse>(new ProblemException("Authorization failed", "Incorrect credentials"))
        );
    }

    private async Task<Result<AuthResponse>> CreateUniqueUserAsync(SignUpCommand command,
        CancellationToken cancellationToken)
    {
        var anyResult = await userRepository.AnyAsync(command.Email, cancellationToken);
        return await anyResult.Match(
            async isAny =>
            {
                if (isAny)
                    return new Result<AuthResponse>(new ProblemException("Registration failed",
                        "User with this email already exists"));

                return await CreateUserAsync(command, cancellationToken);
            },
            exception => Task.FromResult(new Result<AuthResponse>(exception))
        );
    }

    private async Task<Result<AuthResponse>> CreateUserAsync(SignUpCommand command,
        CancellationToken cancellationToken)
    {
        var passwordHasher = new PasswordHasher<SignUpCommand>();
        command.Password = passwordHasher.HashPassword(command, command.Password);

        var userCreationResult = await userRepository.CreateUserAsync(command, cancellationToken);

        return userCreationResult.Match(
            createdUser => EncodeAuthResponse(createdUser),
            exception => new Result<AuthResponse>(exception)
        );
    }

    private AuthResponse EncodeAuthResponse(User user)
    {
        return new AuthResponse
        {
            AccessToken = EncodeAccessToken(user),
            RefreshToken = EncodeRefreshToken(user),
        };
    }

    private string EncodeAccessToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_authOptions.AccessTokenSecret);
        var issuer = _authOptions.AccessTokenIssuer;
        var audience = _authOptions.AccessTokenAudience;
        var expires = DateTime.UtcNow.AddMinutes(_authOptions.AccessTokenTimeToLive);

        return EncodeToken(user, key, issuer, audience, expires);
    }

    private string EncodeRefreshToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_authOptions.RefreshTokenSecret);
        var issuer = _authOptions.RefreshTokenIssuer;
        var audience = _authOptions.RefreshTokenAudience;
        var expires = DateTime.UtcNow.AddDays(_authOptions.RefreshTokenTimeToLive);

        return EncodeToken(user, key, issuer, audience, expires);
    }

    private Result<Guid> DecodeRefreshToken(string refreshToken)
    {
        var key = Encoding.UTF8.GetBytes(_authOptions.RefreshTokenSecret);
        var validator = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = _authOptions.RefreshTokenIssuer,
            ValidateAudience = true,
            ValidAudience = _authOptions.RefreshTokenAudience,
            ClockSkew = TimeSpan.Zero
        };

        if (!validator.CanReadToken(refreshToken))
            return new Result<Guid>(new ProblemException("Authorization refresh failed",
                "Can't read refresh token"));

        try
        {
            var principal = validator.ValidateToken(refreshToken, validationParameters, out _);
            if (!Guid.TryParse(principal.Identities.First().Claims.First(x => x.Type == "userId").Value,
                    out var userId))
                return new Result<Guid>(new ProblemException("Authorization refresh failed",
                    "Can't get userId from refresh token"));
            return userId;
        }
        catch (Exception e)
        {
            return new Result<Guid>(new ProblemException("Authorization refresh failed",
                "Refresh token is not valid"));
        }
    }

    private static string EncodeToken(User user, byte[] key, string issuer, string audience, DateTime expires)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var claims = new ClaimsIdentity([
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("userId", user.Id.ToString()),
            new Claim("username", user.Username),
            new Claim("email", user.Email)
        ]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Issuer = issuer,
            Audience = audience,
            Expires = expires,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}