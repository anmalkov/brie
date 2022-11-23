using Brie.Core.Models;

namespace Brie.Core.Repositories;

public interface IThreatModelsRepository
{
    Task<IEnumerable<ThreatModel>?> GetAllAsync();
}