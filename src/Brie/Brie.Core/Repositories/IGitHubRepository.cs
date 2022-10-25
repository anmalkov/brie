using Brie.Core.Models;

namespace Brie.Core.Repositories;

public interface IGitHubRepository
{
    Task<GitHubDirectory> GetDirectoryAsync(string name, string url);
}