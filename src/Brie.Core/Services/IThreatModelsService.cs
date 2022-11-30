using Brie.Core.Models;

namespace Brie.Core.Services;

public interface IThreatModelsService
{
    Task<IEnumerable<ThreatModel>?> GetAllAsync();
    Task<Category?> GetCategoryAsync();
    Task CreateAsync(ThreatModel threatModel);
    Task DeleteAsync(string id);
    Task<string?> GetReportAsync(string threatModelId);
    Task StoreFileForReportAsync(string threatModelId, string fileName, byte[] content);
}
