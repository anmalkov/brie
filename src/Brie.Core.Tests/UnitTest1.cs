using Brie.Core.Models;
using Brie.Core.Repositories;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Text.RegularExpressions;

namespace Brie.Core.Tests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        var directoryName = "Security Domain";

        var httpClient = new HttpClient();
        var repository = new GitHubApiRepository(httpClient);

        var directory = await repository.GetContentAsync("anmalkov", "brief", directoryName);

        Assert.NotNull(directory);
        Assert.Equal(directoryName, directory.Name);
        Assert.NotNull(directory.Directories);
        Assert.True(directory.Directories.Count() > 0);
    }

    [Fact]
    public async Task Test2()
    {
        var wordTemplate = File.ReadAllBytes("template.docx");
        
        var stream = new MemoryStream();
        stream.Write(wordTemplate, 0, wordTemplate.Length);

        using (var document = WordprocessingDocument.Open(stream, isEditable: true))
        {
            var body = document.MainDocumentPart.Document.Body;
            var tableElement = body.Descendants<Table>().First();
            for (int i = 0; i < 10; i++)
            {
                var row = new TableRow();
                row.Append(new TableCell(new Paragraph(new Run(new Text($"{i}-1")))));
                row.Append(new TableCell(new Paragraph(new Run(new Text($"{i}-2")))));
                row.Append(new TableCell(new Paragraph(new Run(new Text($"{i}-3")))));
                row.Append(new TableCell(new Paragraph(new Run(new Text($"{i}-4")))));
                row.Append(new TableCell(new Paragraph(new Run(new Text($"{i}-5")))));
                tableElement.Append(row);
            }
        }

        using (var document = WordprocessingDocument.Open(stream, isEditable: true))
        {
            string? documentContent = null;
            using (var reader = new StreamReader(document.MainDocumentPart.GetStream()))
            {
                documentContent = await reader.ReadToEndAsync();
            }

            var regex = new Regex(Regex.Escape("[tm-project-name]"));
            documentContent = regex.Replace(documentContent, "Test Project");

            using (var writer = new StreamWriter(document.MainDocumentPart.GetStream(FileMode.Create)))
            {
                await writer.WriteAsync(documentContent);
            }
        }

        using (var document = WordprocessingDocument.Open(stream, isEditable: true))
        {
            var body = document.MainDocumentPart.Document.Body;
            //var bookmark = body.Descendants<BookmarkStart>().First(b => b.Name == "tm_threat_properties");
            var header = body.Descendants<Paragraph>().Where(p => p.Descendants<Run>().Any(r => r.Descendants<Text>().Any(t => t.Text.ToLower() == "threat properties"))).First();

            // hr
            var p1 = new Paragraph(new ParagraphProperties(new ParagraphBorders(new BottomBorder { Val = BorderValues.Single, Color = "auto", Space = 1, Size = 6 })));

            // threat #
            var p2 = new Paragraph();
            var r1 = new Run(new RunProperties(new Bold()));
            r1.Append(new Text("Threat #:"));
            var r2 = new Run(new Text(" 1"));
            r2.Append(new Break());
            p2.Append(r1);
            p2.Append(r2);

            header.InsertAfterSelf(p2);
            header.InsertAfterSelf(p1);
        }

        File.WriteAllBytes("result.docx", stream.ToArray());
    }
}