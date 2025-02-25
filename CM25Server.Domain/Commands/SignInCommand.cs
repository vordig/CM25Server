using CM25Server.Domain.Commands.Validators;
using CM25Server.Domain.Core;

namespace CM25Server.Domain.Commands;

public record SignInCommand : BaseCommand<SignInCommand, SignInCommandValidator>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}