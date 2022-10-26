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

builder.Services.AddScoped<ICategoriesService, CategoriesService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MediateGet<GetCategoriesRequest>("/api/categories");

app.MapFallbackToFile("index.html");

app.Run();
