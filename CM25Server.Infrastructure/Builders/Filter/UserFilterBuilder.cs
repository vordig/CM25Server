using CM25Server.Domain.Models;
using CM25Server.Infrastructure.Core.Builders.Filter;
using CM25Server.Infrastructure.Core.Builders.Filter.Interfaces;

namespace CM25Server.Infrastructure.Builders.Filter;

public class UserFilterBuilder : BaseFilterBuilder<User, UserFilterBuilder>, IIdFilterBuilder<User, UserFilterBuilder>
{
    public UserFilterBuilder WithEmail(string email)
    {
        var filter = Builder.Eq(x => x.Email, email);
        AddFilter(filter);
        return this;
    }
}