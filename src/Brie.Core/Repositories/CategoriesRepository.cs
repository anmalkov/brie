﻿using Brie.Core.Models;
using System.Reflection;
using System.Text.Json;

namespace Brie.Core.Repositories;

public class CategoriesRepository : RepositoryBase<Category>, ICategoriesRepository
{
    private const string RepositoryFilename = "categories.json";

    public CategoriesRepository() : base(RepositoryFilename) { }


    public new async Task<Category?> GetAllAsync()
    {
        var categories = await base.GetAllAsync();
        return categories is not null ? categories.First() : null;
    }

    public new async Task UpdateAllAsync(Category category)
    {
        await base.UpdateAllAsync(new[] { category });
    }
}
