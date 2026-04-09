using Vorchestra.API.Extensions;
using Vorchestra.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddSwagger();
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbService = scope.ServiceProvider.GetRequiredService<IDatabaseService>();

    await dbService.MigrateDatabaseAsync();

    await dbService.SeedDefaultDataAsync();
}

app.UseSwaggerDocs();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
