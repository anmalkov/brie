using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brie.Core.Repositories;

public interface IReportsRepository
{
    Task<byte[]?> GetAsync(string threatModelId, ReportType reportType);
    Task CreateAsync(string threatModelId, string projectName, ReportType reportType, byte[] content);
    Task<bool> StoreAsync(string threatModelId, string fileName, byte[] content);
    void Delete(string threatModelId);
    Task<byte[]?> GetTemplateAsync(ReportType reportType);
    Task StoreTemplateAsync(ReportType reportType, byte[] content);
}
