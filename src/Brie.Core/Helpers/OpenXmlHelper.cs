using Brie.Core.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Brie.Core.Helpers;

public static class OpenXmlHelper
{
    public static async Task ReplaceAsync(Stream stream, string placeholder, string replacement)
    {
        using var document = WordprocessingDocument.Open(stream, isEditable: true);
        string? documentContent = null;
        using (var reader = new StreamReader(document.MainDocumentPart.GetStream()))
        {
            documentContent = await reader.ReadToEndAsync();
        }

        var regex = new Regex(Regex.Escape(placeholder));
        documentContent = regex.Replace(documentContent, replacement);

        using var writer = new StreamWriter(document.MainDocumentPart.GetStream(FileMode.Create));
        await writer.WriteAsync(documentContent);
    }

    public static void AddDataflowAttributes(Stream stream, IEnumerable<DataflowAttribute> attributes)
    {
        using var document = WordprocessingDocument.Open(stream, isEditable: true);

        var tableElement = document.MainDocumentPart.Document.Body.Descendants<Table>().First();

        foreach (var attribute in attributes)
        {
            tableElement.Append(
                new TableRow(new[] {
                    new TableCell(new Paragraph(new Run(new Text(attribute.Number)))),
                    new TableCell(new Paragraph(new Run(new Text(attribute.Transport)))),
                    new TableCell(new Paragraph(new Run(new Text(attribute.DataClassification)))),
                    new TableCell(new Paragraph(new Run(new Text(attribute.Authentication)))),
                    new TableCell(new Paragraph(new Run(new Text(attribute.Notes))))
                })
            );
        }
    }

    public static void AddThreats(Stream stream, IEnumerable<Recommendation> threats)
    {
        using var document = WordprocessingDocument.Open(stream, isEditable: true);
        var body = document.MainDocumentPart.Document.Body;
        
        var header = FindParagraph(body, "Threat Properties");
        if (header is null)
        {
            return;
        }

        var threatIndex = threats.Count();
        foreach (var threat in threats.ToArray().Reverse())
        {
            var paragraphs = new List<Paragraph>
            {
                GetHorizontalLine(),
                new Paragraph(
                    new Run(
                        new RunProperties(new Bold()),
                        new Text("Threat #:")
                    ),
                    new Run(new Text($" {threatIndex}"))
                ),
                new Paragraph(new Run(new Text(threat.Description)))
            };
            foreach (var paragraph in paragraphs.ToArray().Reverse())
            {
                header.InsertAfterSelf(paragraph);
            }
            threatIndex--;
        }
    }

    private static Paragraph? FindParagraph(Body body, string text)
    {
        return body.Descendants<Paragraph>().Where(p => p.Descendants<Run>().Any(r => r.Descendants<Text>().Any(t => t.Text.ToLower() == text.ToLower()))).FirstOrDefault();
    }

    private static Paragraph GetHorizontalLine()
    {
        return new Paragraph(
            new ParagraphProperties(
                new ParagraphBorders(
                    new BottomBorder { Val = BorderValues.Single, Color = "auto", Space = 1, Size = 6 }
                )
            )
        );
    }
}