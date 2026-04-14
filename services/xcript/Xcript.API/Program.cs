using Shared.Application.Contracts;
using Shared.Infrastructure.Extensions;
using Xcript.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddSwagger("Xcript API");
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbService = scope.ServiceProvider.GetRequiredService<IDatabaseService>();

    await dbService.MigrateDatabaseAsync();

    await dbService.SeedDefaultDataAsync();
}

app.UseSwaggerDocs("Xcript API");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
