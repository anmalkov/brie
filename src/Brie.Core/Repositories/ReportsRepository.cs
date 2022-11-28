using Brie.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Brie.Core.Repositories;

public class ReportsRepository : IReportsRepository
{
    private const string RepositoriesDirectoryName = "data";
    private const string ReportsDirectoryName = "reports";
    private const string ReportFileName = "threat-model-report.md";
    private const string TemplateFileName = "template.md";

    private readonly string _reportsFullPath;


    public ReportsRepository()
    {
        _reportsFullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", RepositoriesDirectoryName, ReportsDirectoryName);
    }
    
    
    public async Task CreateAsync(string threatModelId, string projectName, string content)
    {
        var reportDirectory = GetReportDirectoryFullName(threatModelId, projectName);
        if (!Directory.Exists(reportDirectory))
        {
            Directory.CreateDirectory(reportDirectory);
        }
        
        var fileFullName = Path.Combine(reportDirectory, ReportFileName);
        await File.WriteAllTextAsync(fileFullName, content);
    }

    public async Task<bool> StoreAsync(string threatModelId,  string fileName, byte[] content)
    {
        if (!Directory.Exists(_reportsFullPath))
        {
            return false;
        }
        var directories = Directory.GetDirectories(_reportsFullPath, $"*{threatModelId}");
        if (!directories.Any())
        {
            return false;
        }

        var fileFullName = Path.Combine(directories.First(), fileName);
        await File.WriteAllBytesAsync(fileFullName, content);
        return true;
    }

    public async Task<string?> GetAsync(string threatModelId)
    {
        if (!Directory.Exists(_reportsFullPath))
        {
            return null;
        }
        var directories = Directory.GetDirectories(_reportsFullPath, $"*{threatModelId}");
        if (!directories.Any())
        {
            return null;
        }

        var fileFullName = Path.Combine(directories.First(), ReportFileName);
        return await File.ReadAllTextAsync(fileFullName);
    }

    public void Delete(string threatModelId)
    {
        if (!Directory.Exists(_reportsFullPath))
        {
            return;
        }
        
        var directories = Directory.GetDirectories(_reportsFullPath, $"*{threatModelId}");
        if (!directories.Any())
        {
            return;
        }

        Directory.Delete(directories.First(), true);
    }

    public async Task<string?> GetTemplateAsync()
    {
        if (!Directory.Exists(_reportsFullPath))
        {
            return null;
        }

        var templateFileFullName = Path.Combine(_reportsFullPath, TemplateFileName);
        return File.Exists(templateFileFullName) ? await File.ReadAllTextAsync(templateFileFullName) : null;
    }

    public async Task StoreTemplateAsync(string content)
    {
        if (!Directory.Exists(_reportsFullPath))
        {
            Directory.CreateDirectory(_reportsFullPath);
        }

        var templateFileFullName = Path.Combine(_reportsFullPath, TemplateFileName);
        await File.WriteAllTextAsync(templateFileFullName, content);
    }


    private string GetReportDirectoryFullName(string id, string projectName)
    {
        var reportDirectoryName = $"{projectName.Replace(" ", "-").ToLower()}-{id}";
        return Path.Combine(_reportsFullPath, reportDirectoryName);
    }
}
