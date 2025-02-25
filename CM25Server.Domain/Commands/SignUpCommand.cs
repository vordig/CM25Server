using CM25Server.Domain.Commands.Validators;
using CM25Server.Domain.Core;

namespace CM25Server.Domain.Commands;

public record SignUpCommand : BaseCommand<SignUpCommand, SignUpCommandValidator>
{
    public required string Email { get; init; }
    public required string Password { get; set; }
    public required string Username { get; init; }
}