using CM25Server.Domain.Models;
using CM25Server.Services.Contracts;
using Riok.Mapperly.Abstractions;

namespace CM25Server.Services.Mappers;

[Mapper]
public partial class ProjectMapper
{
    [MapNestedProperties(nameof(Project.Audit))]
    public partial ProjectListResponse ToListResponse(Project project);
    
    [MapNestedProperties(nameof(Project.Audit))]
    public partial ProjectDetailResponse ToDetailResponse(Project project);
}