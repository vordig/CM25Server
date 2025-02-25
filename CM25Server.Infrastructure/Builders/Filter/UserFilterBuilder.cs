using CM25Server.Domain.Models;
using CM25Server.Infrastructure.Core.Builders.Filter;

namespace CM25Server.Infrastructure.Builders.Filter;

public class UserFilterBuilder : BaseFilterBuilder<User, UserFilterBuilder>
{
    public UserFilterBuilder WithEmail(string email)
    {
        var filter = Builder.Eq(x => x.Email, email);
        AddFilter(filter);
        return this;
    }
}