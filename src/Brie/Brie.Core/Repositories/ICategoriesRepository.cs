using Brie.Core.Models;

namespace Brie.Core.Repositories;

public interface ICategoriesRepository
{
    Task<Category?> GetAllAsync();
    Task UpdateAllAsync(Category category);
}