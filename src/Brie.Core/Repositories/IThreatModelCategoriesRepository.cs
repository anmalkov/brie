using Brie.Core.Models;

namespace Brie.Core.Repositories;

public interface IThreatModelCategoriesRepository
{
    Task<Category?> GetAllAsync();
    Task UpdateAllAsync(Category category);
}