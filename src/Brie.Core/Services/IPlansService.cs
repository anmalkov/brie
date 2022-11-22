using Brie.Core.Models;

namespace Brie.Core.Services;

public interface IPlansService
{
    Task<IEnumerable<Plan>?> GetAllAsync();
}