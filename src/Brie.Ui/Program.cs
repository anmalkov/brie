using Brie.Core.Repositories;
using Brie.Core.Services;
using Brie.Ui.Extensions;
using Brie.Ui.Requests;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(m => m.AsScoped(), typeof(Program));
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<IGitHubRepository, GitHubRepository>();
builder.Services.AddSingleton<ICategoriesRepository, CategoriesRepository>();
builder.Services.AddSingleton<IThreatModelCategoriesRepository, ThreatModelCategoriesRepository>();
builder.Services.AddSingleton<IThreatModelsRepository, ThreatModelsRepository>();
builder.Services.AddSingleton<IReportsRepository, ReportsRepository>();

builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<IThreatModelsService, ThreatModelsService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MediateGet<GetCategoriesRequest>("/api/categories");

app.MediateGet<GetThreatModelsRequest>("/api/threatmodels");
app.MediateGet<GetThreatModelCategoriesRequest>("/api/threatmodels/categories");
app.MediatePost<CreateThreatModelRequest>("/api/threatmodels");
app.MediateGet<GetThreatModelReportRequest>("/api/threatmodels/{id}/report");
app.MediateDelete<DeleteThreatModelRequest>("/api/threatmodels/{id}");

app.MapFallbackToFile("index.html");

app.Run();
