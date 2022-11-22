using System.Reflection;
using System.Text.Json;

namespace Brie.Core.Repositories;

public class RepositoryBase<T> where T : class
{
    private const string RepositoriesDirectoryName = "data";

    private readonly string _repositoryFullFilename;


    public RepositoryBase(string repositoryFilename)
    {
        var repositoriesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", RepositoriesDirectoryName);
        _repositoryFullFilename = Path.Combine(repositoriesPath, repositoryFilename);
    }


    public async Task<IEnumerable<T>?> GetAllAsync()
    {
        return await LoadAsync();
    }

    public async Task UpdateAllAsync(IEnumerable<T> items)
    {
        await SaveAsync(items ?? throw new ArgumentNullException(nameof(items)));
    }
    
    private async Task<IEnumerable<T>?> LoadAsync()
    {
        if (!File.Exists(_repositoryFullFilename))
        {
            return null;
        }

        var json = await File.ReadAllTextAsync(_repositoryFullFilename);
        return JsonSerializer.Deserialize<IEnumerable<T>>(json);
    }

    private async Task SaveAsync(IEnumerable<T> items)
    {
        var directoryName = Path.GetDirectoryName(_repositoryFullFilename);
        if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        var json = JsonSerializer.Serialize(items);
        await File.WriteAllTextAsync(_repositoryFullFilename, json.ToString());
    }
}
