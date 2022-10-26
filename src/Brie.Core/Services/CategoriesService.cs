﻿using Brie.Core.Models;
using Brie.Core.Repositories;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brie.Core.Services;

public class CategoriesService : ICategoriesService
{
    private const string CategoryCacheKey = "category";

    private readonly IGitHubRepository _gitHubRepository;
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IMemoryCache _memoryCache;

    public CategoriesService(IGitHubRepository gitHubRepository, ICategoriesRepository categoriesRepository, IMemoryCache memoryCache)
    {
        _gitHubRepository = gitHubRepository;
        _categoriesRepository = categoriesRepository;
        _memoryCache = memoryCache;
    }

    public async Task<Category?> GetAsync()
    {
        return await _memoryCache.GetOrCreateAsync(CategoryCacheKey, async entry =>
        {
            entry.SetPriority(CacheItemPriority.NeverRemove);
            var category = await _categoriesRepository.GetAllAsync();
            if (category is null)
            {
                category = await GetRecommendationsFromGitHubAsync();
                await _categoriesRepository.UpdateAllAsync(category);
            }
            return category;
        });
    }
    
    private async Task<Category> GetRecommendationsFromGitHubAsync()
    {
        var directory = await _gitHubRepository.GetContentAsync("anmalkov", "brief", "Security Domain");

        return MapDirectoryToCategory(directory);
    }

    private Category MapDirectoryToCategory(GitHubDirectory directory)
    {
        return new Category(
            directory.Name,
            "",
            directory.Directories?.Select(d => MapDirectoryToCategory(d)).ToList(),
            directory.Files?.Select(f => MapFileToRecommendation(f)).ToList()
        );
    }

    private Recommendation MapFileToRecommendation(GitHubFile file)
    {
        return new Recommendation(
            Path.GetFileNameWithoutExtension(file.Name),
            file.Content
        );
    }
}