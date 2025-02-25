using CM25Server.Domain.Models;
using CM25Server.Services.Contracts;
using Riok.Mapperly.Abstractions;

namespace CM25Server.Services.Mappers;

[Mapper]
public partial class IssueMapper
{
    [MapNestedProperties(nameof(Issue.Audit))]
    public partial IssueListResponse ToListResponse(Issue Issue);
    
    [MapNestedProperties(nameof(Issue.Audit))]
    public partial IssueDetailResponse ToDetailResponse(Issue Issue);
}