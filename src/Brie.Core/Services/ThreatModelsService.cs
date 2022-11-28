using Brie.Core.Models;
using Brie.Core.Repositories;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Brie.Core.Services;

public class ThreatModelsService : IThreatModelsService
{
    private const string CategoryCacheKey = "threatmodels.category";
    private const string MarkdownTemplateCacheKey = "threatmodels.template";
    private const string ProjectNamePlaceholder = "[tm-project-name]";
    private const string DataflowAttributesPlaceholder = "[tm-data-flow-attributes]";
    private const string ThreatPropertiesPlaceholder = "[tm-threat-properties]";

    private readonly IGitHubRepository _gitHubRepository;
    private readonly IThreatModelsRepository _threatModelsRepository;
    private readonly IThreatModelCategoriesRepository _threatModelCategoriesRepository;
    private readonly IMemoryCache _memoryCache;
    private readonly IReportsRepository _reportsRepository;

    public ThreatModelsService(IGitHubRepository gitHubRepository, IThreatModelsRepository threatModelsRepository, 
        IThreatModelCategoriesRepository threatModelCategoriesRepository, IMemoryCache memoryCache, IReportsRepository reportsRepository)
    {
        _gitHubRepository = gitHubRepository;
        _threatModelsRepository = threatModelsRepository;
        _threatModelCategoriesRepository = threatModelCategoriesRepository;
        _memoryCache = memoryCache;
        _reportsRepository = reportsRepository;
    }

    
    public async Task<IEnumerable<ThreatModel>?> GetAllAsync()
    {
        return await _threatModelsRepository.GetAllAsync();
    }

    public async Task<Category?> GetCategoryAsync()
    {
        return await _memoryCache.GetOrCreateAsync(CategoryCacheKey, async entry =>
        {
            entry.SetPriority(CacheItemPriority.NeverRemove);
            var category = await _threatModelCategoriesRepository.GetAllAsync();
            if (category is null)
            {
                category = await GetRecommendationsFromGitHubAsync();
                await _threatModelCategoriesRepository.UpdateAllAsync(category);
            }
            return category;
        });
    }

    public async Task CreateAsync(ThreatModel threatModel)
    {
        await _threatModelsRepository.CreateAsync(threatModel);
        await GenerateAndSaveReportAsync(threatModel);
    }

    public async Task<string?> GetReportAsync(string threatModelId)
    {
        var report = await _reportsRepository.GetAsync(threatModelId);
        if (report is not null)
        {
            return report;
        }

        var threatModel = await _threatModelsRepository.GetAsync(threatModelId);
        if (threatModel is null)
        {
            return null;
        }

        return await GenerateAndSaveReportAsync(threatModel);
    }

    public async Task StoreFileForReportAsync(string threatModelId, string fileName, byte[] content)
    {
        await _reportsRepository.StoreAsync(threatModelId, fileName, content);
    }

    public async Task DeleteAsync(string id)
    {
        await _threatModelsRepository.DeleteAsync(id);
        _reportsRepository.Delete(id);
    }


    private async Task<string?> GenerateAndSaveReportAsync(ThreatModel threatModel)
    {
        var mdReport = await GenerateReportAsync(threatModel);
        if (mdReport is not null)
        {
            await _reportsRepository.CreateAsync(threatModel.Id, threatModel.ProjectName, mdReport);
        }
        
        return mdReport;
    }

    private async Task<string?> GenerateReportAsync(ThreatModel threadModel)
    {
        var mdReport = await _memoryCache.GetOrCreateAsync(MarkdownTemplateCacheKey, async entry =>
        {
            entry.SetAbsoluteExpiration(TimeSpan.FromHours(24));
            var file = await _gitHubRepository.GetFileAsync("anmalkov", "brief", "Threat Model/threat-model-template.md");
            return file.Content;
        });
        if (mdReport is null)
        {
            return null;
        }
        
        mdReport = mdReport.Replace(ProjectNamePlaceholder, threadModel.ProjectName);
        var dataflowAttributeSection = GenerateDataflowAttributeSection(threadModel);
        mdReport = mdReport.Replace(DataflowAttributesPlaceholder, dataflowAttributeSection);
        var threatModelPropertiesSection = GenerateThreatModelPropertiesSection(threadModel);
        mdReport = mdReport.Replace(ThreatPropertiesPlaceholder, threatModelPropertiesSection);
        return mdReport;
    }
        
    private static string GenerateThreatModelPropertiesSection(ThreatModel threadModel)
    {
        var section = new StringBuilder();
        var index = 1;
        foreach (var threat in threadModel.Threats)
        {
            if (index > 1)
            {
                section.AppendLine();
            }
            section.AppendLine("---");
            section.AppendLine($"**Threat #:** {index}  ");
            section.AppendLine(threat.Description.Trim());
            index++;
        }
        return section.ToString().TrimEnd(Environment.NewLine.ToCharArray());
    }

    private static string GenerateDataflowAttributeSection(ThreatModel threadModel)
    {
        var section = new StringBuilder();
        foreach (var a in threadModel.DataflowAttributes)
        {
            section.AppendLine($"| {a.Number.Trim()} | {a.Transport.Trim()} | {a.DataClassification.Trim()} | {a.Authentication.Trim()} | {a.Notes.Trim()} |");
        }
        return section.ToString().TrimEnd(Environment.NewLine.ToCharArray());
    }

    private async Task<Category> GetRecommendationsFromGitHubAsync()
    {
        var directory = await _gitHubRepository.GetContentAsync("anmalkov", "brief", "Threat Model");

        return MapDirectoryToCategory(directory);
    }

    private Category MapDirectoryToCategory(GitHubDirectory directory)
    {
        return new Category(
            GenerateIdFor(directory.Url),
            directory.Name,
            "",
            directory.Directories?.Select(d => MapDirectoryToCategory(d)).ToList(),
            directory.Files?.Where(f => f.Name != "threat-model-template.md").Select(f => MapFileToRecommendation(f)).ToList()
        );
    }

    private static string GenerateIdFor(string text)
    {
        return string.Join("", (SHA1.HashData(Encoding.UTF8.GetBytes(text))).Select(b => b.ToString("x2")));
    }

    private Recommendation MapFileToRecommendation(GitHubFile file)
    {
        return new Recommendation(
            GenerateIdFor(file.Url),
            Path.GetFileNameWithoutExtension(file.Name),
            file.Content
        );
    }

}
