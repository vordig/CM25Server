using CM25Server.Domain.Models;
using CM25Server.Services.Contracts.Responses;
using Riok.Mapperly.Abstractions;

namespace CM25Server.Services.Mappers;

[Mapper]
public partial class ProjectMapper
{
    [MapNestedProperties(nameof(Project.Audit))]
    public partial ProjectResponse ToResponse(Project project);
}