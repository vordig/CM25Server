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
                    PasswordVerificationResult.Success => new AuthResponse
                    {
                        AccessToken = EncodeAccessToken(existingUser)
                    },
                    PasswordVerificationResult.SuccessRehashNeeded => new AuthResponse
                    {
                        AccessToken = EncodeAccessToken(existingUser)
                    },
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
            createdUser => new AuthResponse
            {
                AccessToken = EncodeAccessToken(createdUser)
            },
            exception => new Result<AuthResponse>(exception)
        );
    }

    private string EncodeAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_authOptions.JWTSecret);

        var claims = new ClaimsIdentity([
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        ]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Issuer = _authOptions.JWTIssuer,
            Audience = _authOptions.JWTAudience,
            Expires = DateTime.UtcNow.AddMinutes(_authOptions.JWTTimeToLive),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}