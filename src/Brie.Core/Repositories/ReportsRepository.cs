using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Brie.Core.Repositories;

public class ReportsRepository : IReportsRepository
{
    private const string RepositoriesDirectoryName = "data";
    private const string ReportsDirectoryName = "reports";

    private readonly string _reportsFullPath;


    public ReportsRepository()
    {
        _reportsFullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", RepositoriesDirectoryName, ReportsDirectoryName);
    }
    
    
    public async Task CreateAsync(string id, string projectName, string content)
    {
        if (!Directory.Exists(_reportsFullPath))
        {
            Directory.CreateDirectory(_reportsFullPath);
        }
        await File.WriteAllTextAsync(GetReportFullFileName(id, projectName), content);
    }

    public async Task<string> GetAsync(string id)
    {
        var files = Directory.GetFiles(_reportsFullPath, $"*{id}.md");
        if (files.Any())
        {
            return await File.ReadAllTextAsync(files.First());
        }
        
        return "";
    }

    
    private string GetReportFullFileName(string id, string projectName)
    {
        projectName = projectName.Replace(" ", "-").ToLower();
        return Path.Combine(_reportsFullPath, $"threat-model-{projectName}-{id}.md");
    }
}
