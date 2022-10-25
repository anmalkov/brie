using Brie.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Brie.Core.Repositories;

public class GitHubRepository : IGitHubRepository
{
    private readonly HttpClient _httpClient;

    private record GitHubDto(string Name, string Url, string Type, string? Content);

    public GitHubRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "brief");
        _httpClient.DefaultRequestHeaders.Add("Host", "api.github.com");
    }

    //public async Task<GitHubDirectory> GetDirectoryAsync()
    //{
    //    return await GetDirectoryAsync("Security Domain", "https://api.github.com/repos/anmalkov/brief/contents/Security%20Domain?ref=main");
    //}

    private async Task<GitHubDirectory> GetDirectoryAsync(string name, string url)
    {
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Unknown response from GitHub. Status Code: ${response.StatusCode}");
        }

        var dtos = await response.Content.ReadFromJsonAsync<IEnumerable<GitHubDto>>();
        var files = dtos!.Where(f => f.Type == "file").Select(async f => await GetFileAsync(f.Url)).Select(t => t.Result).ToList();
        var directories = dtos!.Where(d => d.Type == "dir").Select(async d => await GetDirectoryAsync(d.Name, d.Url)).Select(t => t.Result).ToList();
        return new GitHubDirectory(
            name,
            directories,
            files
        );
    }

    private async Task<GitHubFile> GetFileAsync(string url)
    {
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Unknown response from GitHub. Status Code: ${response.StatusCode}");
        }

        var dto = await response.Content.ReadFromJsonAsync<GitHubDto>();
        var content = Encoding.UTF8.GetString(Convert.FromBase64String(dto!.Content ?? ""));
        return new GitHubFile(dto!.Name, content);
    }
}
