using CM25Server.Domain.Models;
using CM25Server.Infrastructure.Core.Builders.Filter;
using CM25Server.Infrastructure.Core.Builders.Filter.Interfaces;

namespace CM25Server.Infrastructure.Builders.Filter;

public class RefreshTokenFilterBuilder : BaseFilterBuilder<RefreshToken, RefreshTokenFilterBuilder>,
    IIdFilterBuilder<RefreshToken, RefreshTokenFilterBuilder>,
    IOwnedByUserFilterBuilder<RefreshToken, RefreshTokenFilterBuilder>
{
}