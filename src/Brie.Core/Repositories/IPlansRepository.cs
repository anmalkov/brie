using Brie.Core.Models;

namespace Brie.Core.Repositories;

public interface IPlansRepository
{
    Task<IEnumerable<Plan>?> GetAllAsync();
}