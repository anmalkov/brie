using Brie.Core.Models;
using System.Reflection;
using System.Text.Json;

namespace Brie.Core.Repositories;

public class CategoriesRepository : ICategoriesRepository
{
    private const string RepositoriedDirectoryName = "data";
    private const string RepositoryFilename = "categories.json";

    private readonly string _repositoryFullFilename;


    public CategoriesRepository()
    {
        var repositoriesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", RepositoriedDirectoryName);
        _repositoryFullFilename = Path.Combine(repositoriesPath, RepositoryFilename);
    }


    public async Task<Category?> GetAllAsync()
    {
        return await LoadAsync();
    }

    public async Task UpdateAllAsync(Category category)
    {
        await SaveAsync(category ?? throw new ArgumentNullException(nameof(category)));
    }

    private async Task<Category?> LoadAsync()
    {
        if (!File.Exists(_repositoryFullFilename))
        {
            return null;
        }

        var json = await File.ReadAllTextAsync(_repositoryFullFilename);
        return JsonSerializer.Deserialize<Category>(json);
    }

    private async Task SaveAsync(Category category)
    {
        var directoryName = Path.GetDirectoryName(_repositoryFullFilename);
        if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        var json = JsonSerializer.Serialize(category);
        await File.WriteAllTextAsync(_repositoryFullFilename, json.ToString());
    }
}
