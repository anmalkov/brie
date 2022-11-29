using Brie.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Brie.Core.Repositories;

public class GitHubGitRepository : IGitHubRepository
{
    private const string GitHubRepositoriesDirectoryName = "repos";

    private readonly string _repositoriesFullPath;

    private record GitHubDto(string Name, string Url, string Type, string? Content);

    public GitHubGitRepository()
    {
        _repositoriesFullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", GitHubRepositoriesDirectoryName);
    }

    public Task<GitHubDirectory> GetContentAsync(string accountName, string repositoryName, string folderName)
    {
        if (string.IsNullOrWhiteSpace(accountName))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(accountName));
        }
        if (string.IsNullOrWhiteSpace(repositoryName))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(repositoryName));
        }
        if (string.IsNullOrWhiteSpace(folderName))
        {
            folderName = ".";
        }

        if (!RepositoryCloned())
        {
            CloneRepository(accountName, repositoryName);
        }

        var directoryPath = Path.Combine(_repositoriesFullPath, folderName);
        var directoryName = folderName.Split(Path.DirectorySeparatorChar).Last();
        return Task.FromResult(GetDirectory(directoryName, directoryPath));
    }

    public async Task<GitHubFile> GetFileAsync(string accountName, string repositoryName, string path)
    {
        if (!RepositoryCloned())
        {
            CloneRepository(accountName, repositoryName);
        }

        var filePath = Path.Combine(_repositoriesFullPath, path);
        return await GetFileAsync(filePath);
    }

    
    private void CloneRepository(string accountName, string repositoryName)
    {
        if (!Directory.Exists(_repositoriesFullPath))
        {
            Directory.CreateDirectory(_repositoriesFullPath);
        }
        
        var repositoryUrl = $"https://github.com/{accountName}/{repositoryName}.git";
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = $"clone {repositoryUrl} {_repositoriesFullPath}",
            }
        };
        process.Start();
    }

    private bool RepositoryCloned()
    {
        if (!Directory.Exists(_repositoriesFullPath))
        {
            return false;
        }

        var directories = Directory.GetDirectories(_repositoriesFullPath);
        return directories.Any();
    }

    private GitHubDirectory GetDirectory(string name, string path)
    {
        var files = Directory.GetFiles(path).Select(async p => await GetFileAsync(p)).Select(t => t.Result).ToArray();
        var directories = Directory.GetDirectories(path).Select(p => GetDirectory(p.Split(Path.DirectorySeparatorChar).Last(), p)).ToArray();
        return new GitHubDirectory(
            name,
            path,
            directories,
            files
        );
    }

    private static async Task<GitHubFile> GetFileAsync(string path)
    {
        var isMarkdownFile = Path.GetExtension(path).ToLower() == ".md";
        return new GitHubFile(
            Path.GetFileName(path),
            path,
            isMarkdownFile ? await File.ReadAllTextAsync(path) : null,
            isMarkdownFile ? null : await File.ReadAllBytesAsync(path)
        );
    }
}
