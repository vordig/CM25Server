using CM25Server.Domain.Models;
using Riok.Mapperly.Abstractions;

namespace CM25Server.Domain.Commands.Mappers;

[Mapper]
public partial class SignUpCommandMapper
{
    public partial User ToUser(SignUpCommand command);
}