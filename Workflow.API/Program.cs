using Shared.Application.Contracts;
using Shared.Infrastructure.Extensions;
using Workflow.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCorsPolicy();
builder.Services.AddControllers();
builder.Services.AddSwagger("Workflow API");
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbService = scope.ServiceProvider.GetRequiredService<IDatabaseService>();
    await dbService.MigrateDatabaseAsync();
    await dbService.SeedDefaultDataAsync();
}

app.UseSwaggerDocs("Workflow API");

app.UseHttpsRedirection();

app.UseCorsPolicy();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
