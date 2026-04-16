using Shared.Application.Contracts;
using Shared.Infrastructure.Extensions;
using Vorchestra.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCorsPolicy();
builder.Services.AddControllers();
builder.Services.AddSwagger("Vorchestra API");
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbService = scope.ServiceProvider.GetRequiredService<IDatabaseService>();

    await dbService.MigrateDatabaseAsync();

    await dbService.SeedDefaultDataAsync();
}

app.UseSwaggerDocs("Vorchestra API");

app.UseHttpsRedirection();

app.UseCorsPolicy();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
