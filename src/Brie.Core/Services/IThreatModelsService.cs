using Brie.Core.Models;

namespace Brie.Core.Services;

public interface IThreatModelsService
{
    Task<IEnumerable<ThreatModel>?> GetAllAsync();
    Task<Category?> GetCategoryAsync();
    Task CreateAsync(ThreatModel threadModel);
    Task<string> GetReportAsync(string id);
}