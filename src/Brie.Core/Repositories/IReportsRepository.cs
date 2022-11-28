using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brie.Core.Repositories;

public interface IReportsRepository
{
    Task<string?> GetAsync(string threatModelId);
    Task CreateAsync(string threatModelId, string projectName, string content);
    Task<bool> StoreAsync(string threatModelId, string fileName, byte[] content);
    void Delete(string threatModelId);
}
