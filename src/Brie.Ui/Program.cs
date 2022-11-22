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
builder.Services.AddSingleton<IPlansRepository, PlansRepository>();

builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<IPlansService, PlansService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MediateGet<GetCategoriesRequest>("/api/categories");
app.MediateGet<GetPlansRequest>("/api/plans");

app.MapFallbackToFile("index.html");

app.Run();
