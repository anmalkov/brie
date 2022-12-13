using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brie.Core.Repositories;

public interface IReportsRepository
{
    Task<byte[]?> GetAsync(string threatModelId, ReportType reportType);
    Task<(byte[]? archiveContent, string fileName)> GetArchiveAsync(string threatModelId);
    Task CreateAsync(string threatModelId, string projectName, ReportType reportType, byte[] content);
    Task<bool> StoreAsync(string threatModelId, string fileName, byte[] content);
    bool Exists(string threatModelId, ReportType reportType);
    void Delete(string threatModelId);
    Task<byte[]?> GetTemplateAsync(ReportType reportType);
    Task StoreTemplateAsync(ReportType reportType, byte[] content);
}
