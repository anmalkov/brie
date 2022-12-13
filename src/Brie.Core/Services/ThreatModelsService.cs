using Brie.Core.Models;
using Brie.Core.Repositories;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;
using Category = Brie.Core.Models.Category;
using Brie.Core.Helpers;

namespace Brie.Core.Services;

public class ThreatModelsService : IThreatModelsService
{
    private const string GitHubAccountName = "anmalkov";
    private const string GitHubRepositoryName = "brief";
    private const string GitHubThreatModelFolderName = "Threat Model";
    private const string GitHubMarkdownThreatModelTemplateFilePath = GitHubThreatModelFolderName + "/threat-model-template.md";
    private const string GitHubWordThreatModelTemplateFilePath = GitHubThreatModelFolderName + "/threat-model-template.docx";

    private const string CategoryCacheKey = "threatmodels.category";
    private const string MarkdownTemplateCacheKey = "threatmodels.template";
    private const string WordTemplateCacheKey = "threatmodels.word-template";
    private const string ProjectNamePlaceholder = "[tm-project-name]";
    private const string DataflowAttributesPlaceholder = "[tm-data-flow-attributes]";
    private const string ThreatPropertiesPlaceholder = "[tm-threat-properties]";
    private const string ImagesPlaceholderPrefix = "tm-image-";
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
        await GenerateAndSaveReportsAsync(threatModel);
    }

    public async Task<string?> GetReportAsync(string threatModelId)
    {
        if (_reportsRepository.Exists(threatModelId, ReportType.Markdown))
        {
            var reportContent = await _reportsRepository.GetAsync(threatModelId, ReportType.Markdown);
            return Encoding.UTF8.GetString(reportContent!);
        }

        return await GenerateAndSaveReportsAsync(threatModelId);
    }

    public async Task<(byte[]? archiveContent, string fileName)> GetReportArchiveAsync(string threatModelId)
    {
        if (!_reportsRepository.Exists(threatModelId, ReportType.Markdown))
        {
            await GenerateAndSaveReportsAsync(threatModelId);
        }
        return await _reportsRepository.GetArchiveAsync(threatModelId);
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


    private async Task<string?> GenerateAndSaveReportsAsync(string threatModelId)
    {
        var threatModel = await _threatModelsRepository.GetAsync(threatModelId);
        if (threatModel is null)
        {
            return null;
        }

        return await GenerateAndSaveReportsAsync(threatModel);
    }

    private async Task<string?> GenerateAndSaveReportsAsync(ThreatModel threatModel)
    {
        var mdReport = await GenerateMarkdownReportAsync(threatModel);
        if (mdReport is not null)
        {
            await _reportsRepository.CreateAsync(threatModel.Id, threatModel.ProjectName, ReportType.Markdown, Encoding.UTF8.GetBytes(mdReport));
        }

        var wordReport = await GenerateWordReportAsync(threatModel);
        if (wordReport is not null)
        {
            await _reportsRepository.CreateAsync(threatModel.Id, threatModel.ProjectName, ReportType.Word, wordReport);
        }

        return mdReport;
    }

    private async Task<string?> GenerateMarkdownReportAsync(ThreatModel threatModel)
    {
        var mdReport = await _memoryCache.GetOrCreateAsync(MarkdownTemplateCacheKey, async entry =>
        {
            var template = "";
            entry.SetAbsoluteExpiration(TimeSpan.FromHours(24));
            var templateContent = await _reportsRepository.GetTemplateAsync(ReportType.Markdown);
            if (templateContent is not null)
            {
                template = Encoding.UTF8.GetString(templateContent);
            }
            else
            {
                var file = await _gitHubRepository.GetFileAsync(GitHubAccountName, GitHubRepositoryName, GitHubMarkdownThreatModelTemplateFilePath);
                template = file.Content;
                await _reportsRepository.StoreTemplateAsync(ReportType.Markdown, Encoding.UTF8.GetBytes(template));
            }
            return template;
        });
        if (string.IsNullOrWhiteSpace(mdReport))
        {
            return null;
        }
        
        mdReport = mdReport.Replace(ProjectNamePlaceholder, threatModel.ProjectName);
        var dataflowAttributeSection = GenerateDataflowAttributeSection(threatModel);
        mdReport = mdReport.Replace(DataflowAttributesPlaceholder, dataflowAttributeSection);
        var threatModelPropertiesSection = GenerateThreatModelPropertiesSection(threatModel);
        mdReport = mdReport.Replace(ThreatPropertiesPlaceholder, threatModelPropertiesSection);
        if (threatModel.Images is not null)
        {
            foreach (var image in threatModel.Images)
            {
                mdReport = mdReport.Replace($"[{ImagesPlaceholderPrefix}{image.Key}]", $"![{image.Key}](./{image.Value})");
            }
        }
        return mdReport;
    }

    private async Task<byte[]?> GenerateWordReportAsync(ThreatModel threatModel)
    {
        var wordTemplate = await _memoryCache.GetOrCreateAsync(WordTemplateCacheKey, async entry =>
        {
            entry.SetAbsoluteExpiration(TimeSpan.FromHours(24));
            var templateContent = await _reportsRepository.GetTemplateAsync(ReportType.Word);
            if (templateContent is null)
            { 
                var file = await _gitHubRepository.GetFileAsync(GitHubAccountName, GitHubRepositoryName, GitHubWordThreatModelTemplateFilePath);
                templateContent = file.BinaryContent;
                await _reportsRepository.StoreTemplateAsync(ReportType.Word, templateContent);
            }
            return templateContent;
        });
        if (wordTemplate is null)
        {
            return null;
        }

        var stream = new MemoryStream();
        stream.Write(wordTemplate, 0, wordTemplate.Length);

        await OpenXmlHelper.ReplaceAsync(stream, ProjectNamePlaceholder, threatModel.ProjectName);
        OpenXmlHelper.AddDataflowAttributes(stream, threatModel.DataflowAttributes);
        OpenXmlHelper.AddThreats(stream, threatModel.Threats);

        return stream.ToArray();
    }

    private static string GenerateThreatModelPropertiesSection(ThreatModel threatModel)
    {
        var section = new StringBuilder();
        var index = 1;
        foreach (var threat in threatModel.Threats)
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

    private static string GenerateDataflowAttributeSection(ThreatModel threatModel)
    {
        var section = new StringBuilder();
        foreach (var a in threatModel.DataflowAttributes)
        {
            section.AppendLine($"| {a.Number.Trim()} | {a.Transport.Trim()} | {a.DataClassification.Trim()} | {a.Authentication.Trim()} | {a.Notes.Trim()} |");
        }
        return section.ToString().TrimEnd(Environment.NewLine.ToCharArray());
    }

    private async Task<Category> GetRecommendationsFromGitHubAsync()
    {
        var directory = await _gitHubRepository.GetContentAsync(GitHubAccountName, GitHubRepositoryName, GitHubThreatModelFolderName);

        return MapDirectoryToCategory(directory);
    }

    private Category MapDirectoryToCategory(GitHubDirectory directory)
    {
        return new Category(
            GenerateIdFor(directory.Url),
            directory.Name,
            "",
            directory.Directories?.Select(d => MapDirectoryToCategory(d)).ToList(),
            directory.Files?.Where(f => f.Name != "threat-model-template.md" && f.Name != "threat-model-template.docx").Select(f => MapFileToRecommendation(f)).ToList()
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
            System.IO.Path.GetFileNameWithoutExtension(file.Name),
            file.Content
        );
    }

}
