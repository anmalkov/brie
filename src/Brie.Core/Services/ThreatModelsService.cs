using Brie.Core.Models;
using Brie.Core.Repositories;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Brie.Core.Services;

public class ThreatModelsService : IThreatModelsService
{
    private const string CategoryCacheKey = "threatmodels.category";

    private readonly IGitHubRepository _gitHubRepository;
    private readonly IThreatModelsRepository _threatModelsRepository;
    private readonly IThreatModelCategoriesRepository _threatModelCategoriesRepository;
    private readonly IMemoryCache _memoryCache;

    
    public ThreatModelsService(IGitHubRepository gitHubRepository, IThreatModelsRepository threatModelsRepository, IThreatModelCategoriesRepository threatModelCategoriesRepository, IMemoryCache memoryCache)
    {
        _gitHubRepository = gitHubRepository;
        _threatModelsRepository = threatModelsRepository;
        _threatModelCategoriesRepository = threatModelCategoriesRepository;
        _memoryCache = memoryCache;
    }

    
    public async Task<IEnumerable<ThreatModel>?> GetAllAsync()
    {
        return await _threatModelsRepository.GetAllAsync();
    }

    public async Task<Category?> GetCategoryAsync()
    {
        return await _memoryCache.GetOrCreateAsync(CategoryCacheKey, async entry =>
        {
            entry.SetPriority(CacheItemPriority.NeverRemove);
            var category = await _threatModelCategoriesRepository.GetAllAsync();
            if (category is null)
            {
                category = await GetRecommendationsFromGitHubAsync();
                await _threatModelCategoriesRepository.UpdateAllAsync(category);
            }
            return category;
        });
    }

    public async Task CreateAsync(ThreatModel threadModel)
    {
        await _threatModelsRepository.CreateAsync(threadModel);
    }

    private async Task<Category> GetRecommendationsFromGitHubAsync()
    {
        var directory = await _gitHubRepository.GetContentAsync("anmalkov", "brief", "Threat Model");

        return MapDirectoryToCategory(directory);
    }

    private Category MapDirectoryToCategory(GitHubDirectory directory)
    {
        return new Category(
            GenerateIdFor(directory.Url),
            directory.Name,
            "",
            directory.Directories?.Select(d => MapDirectoryToCategory(d)).ToList(),
            directory.Files?.Where(f => f.Name != "threat-model-template.md").Select(f => MapFileToRecommendation(f)).ToList()
        );
    }

    private static string GenerateIdFor(string text)
    {
        return string.Join("", (SHA1.HashData(Encoding.UTF8.GetBytes(text))).Select(b => b.ToString("x2")));
    }

    private Recommendation MapFileToRecommendation(GitHubFile file)
    {
        return new Recommendation(
            GenerateIdFor(file.Url),
            Path.GetFileNameWithoutExtension(file.Name),
            file.Content
        );
    }
}
