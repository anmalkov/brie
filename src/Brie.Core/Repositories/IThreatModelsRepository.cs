using Brie.Core.Models;

namespace Brie.Core.Repositories;

public interface IThreatModelsRepository
{
    Task<IEnumerable<ThreatModel>?> GetAllAsync();
    Task<ThreatModel?> GetAsync(string id);
    Task CreateAsync(ThreatModel item);
}