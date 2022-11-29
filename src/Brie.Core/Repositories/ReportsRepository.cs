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

public enum ReportType
{
    Markdown = 1,
    Word
};

public class ReportsRepository : IReportsRepository
{
    private const string RepositoriesDirectoryName = "data";
    private const string ReportsDirectoryName = "reports";
    private const string MarkdownReportFileName = "threat-model-report.md";
    private const string WordReportFileName = "threat-model-report.docx";
    private const string MarkdownTemplateFileName = "template.md";
    private const string WordTemplateFileName = "template.docx";

    private readonly string _reportsFullPath;


    public ReportsRepository()
    {
        _reportsFullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", RepositoriesDirectoryName, ReportsDirectoryName);
    }
    
    
    public async Task CreateAsync(string threatModelId, string projectName, ReportType reportType, byte[] content)
    {
        var reportDirectory = GetReportDirectoryFullName(threatModelId, projectName);
        if (!Directory.Exists(reportDirectory))
        {
            Directory.CreateDirectory(reportDirectory);
        }

        var fileName = GetReportFileName(reportType);
        var fileFullName = Path.Combine(reportDirectory, fileName);
        await File.WriteAllBytesAsync(fileFullName, content);
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

    public async Task<byte[]?> GetAsync(string threatModelId, ReportType reportType)
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

        var fileName = GetReportFileName(reportType);
        var fileFullName = Path.Combine(directories.First(), fileName);
        return await File.ReadAllBytesAsync(fileFullName);
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

    public async Task<byte[]?> GetTemplateAsync(ReportType reportType)
    {
        if (!Directory.Exists(_reportsFullPath))
        {
            return null;
        }

        var fileName = GetTemplateFileName(reportType);
        var templateFileFullName = Path.Combine(_reportsFullPath, fileName);
        return File.Exists(templateFileFullName) ? await File.ReadAllBytesAsync(templateFileFullName) : null;
    }

    public async Task StoreTemplateAsync(ReportType reportType, byte[] content)
    {
        if (!Directory.Exists(_reportsFullPath))
        {
            Directory.CreateDirectory(_reportsFullPath);
        }

        var fileName = GetTemplateFileName(reportType);
        var templateFileFullName = Path.Combine(_reportsFullPath, fileName);
        await File.WriteAllBytesAsync(templateFileFullName, content);
    }

    private static string GetTemplateFileName(ReportType reportType)
    {
        return reportType switch
        {
            ReportType.Word => WordTemplateFileName,
            _ => MarkdownTemplateFileName
        };
    }

    private static string GetReportFileName(ReportType reportType)
    {
        return reportType switch
        {
            ReportType.Word => WordReportFileName,
            _ => MarkdownReportFileName
        };
    }

    private string GetReportDirectoryFullName(string id, string projectName)
    {
        var reportDirectoryName = $"{projectName.Replace(" ", "-").ToLower()}-{id}";
        return Path.Combine(_reportsFullPath, reportDirectoryName);
    }
}
