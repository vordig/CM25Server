using CM25Server.Services;
using CM25Server.WebApi.ApiVersioning;
using CM25Server.WebApi.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddDefaultApiVersioning();

builder.Services.AddSingleton<ProjectService>();
builder.Services.AddSingleton<IssueService>();

var app = builder.Build();

app.UseDefaultApiVersioning();

var apiVersionManager = app.Services.GetRequiredService<ApiVersionManager>();
app.UseEndpoints(apiVersionManager);

app.Run();

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);
