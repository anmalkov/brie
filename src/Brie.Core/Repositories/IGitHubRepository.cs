using Brie.Core.Models;

namespace Brie.Core.Repositories;

public interface IGitHubRepository
{
    Task<GitHubDirectory> GetContentAsync(string accountName, string repositoryName, string folderName);
    Task<GitHubFile> GetFileAsync(string accountName, string repositoryName, string path);
}