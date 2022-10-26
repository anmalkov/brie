using Brie.Core.Models;

namespace Brie.Core.Services;

public interface ICategoriesService
{
    Task<Category?> GetAsync();
}