using CM25Server.Domain.Commands;
using CM25Server.Domain.Core;

namespace CM25Server.Domain.Models;

public class User : BaseModel
{
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }

    public static User FromCommand(SignUpCommand command)
    {
        var result = new User
        {
            Email = command.Email,
            Username = command.Username,
            Password = command.Password
        };
        return result;
    }
}