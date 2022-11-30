﻿using Brie.Core.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;

namespace Brie.Core.Helpers;

public static class OpenXmlHelper
{
    private enum ParagraphPartType
    {
        Text,
        Link,
        Style,
    }

    private record ParagraphPart(
        ParagraphPartType Type,
        string? Text,
        string? Style
    );

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
                    new Run(new Text($" {threatIndex}") { Space = SpaceProcessingModeValues.Preserve })
                )
            };
            paragraphs.AddRange(GetParagraphsFromMarkdown(threat.Description));
            foreach (var paragraph in paragraphs.ToArray().Reverse())
            {
                var hyperlinks = paragraph.Descendants<Hyperlink>();
                foreach (var hyperlink in hyperlinks)
                {
                    var uri = new Uri(hyperlink.DocLocation);
                    var relationship = document.MainDocumentPart.AddHyperlinkRelationship(uri, true);
                    hyperlink.Id = relationship.Id;
                    hyperlink.DocLocation = "";
                }
                header.InsertAfterSelf(paragraph);
            }
            threatIndex--;
        }
    }

    
    private static IEnumerable<Paragraph> GetParagraphsFromMarkdown(string markdown)
    {
        var paragraphs = new List<Paragraph>();
        var lines = markdown.Split(Environment.NewLine);
        foreach (var line in lines)
        {
            var text = line;
            var paragraph = new Paragraph();
            if (line.StartsWith('#')) {
                var level = line.TakeWhile(c => c == '#').Count();
                paragraph.Append(new ParagraphProperties(new ParagraphStyleId { Val = $"Heading{level}" }));
                text = line[level..];
            }

            var parts = new List<ParagraphPart>();
            var styles = new Stack<string>();
            var currentTextPart = "";
            for (int i = 0; i < text.Length; i++)
            {
                char? previousCharacter = i > 0 ? text[i - 1] : null;
                var currentCharacter = text[i];
                char? nextCharacter = i < text.Length - 1 ? text[i + 1] : null;
                
                if (currentCharacter == '[' && (previousCharacter is null || previousCharacter != '\\'))
                {
                    var regex = new Regex(@"\[(?<text>[^\]]+)\]\((?<url>[^\)]+)\)");
                    var match = regex.Match(text[i..]);
                    if (match.Success && match.Index == 0)
                    {
                        if (!string.IsNullOrEmpty(currentTextPart))
                        {
                            parts.Add(new ParagraphPart(ParagraphPartType.Text, currentTextPart, null));
                            currentTextPart = "";
                        }
                        parts.Add(new ParagraphPart(ParagraphPartType.Link, match.Groups["text"].Value, match.Groups["url"].Value));
                        i += match.Length;
                    }
                } 
                else if ((currentCharacter == '`' || currentCharacter == '*' || currentCharacter == '_') && (previousCharacter is null || previousCharacter != '\\'))
                {
                    if (!string.IsNullOrEmpty(currentTextPart))
                    {
                        parts.Add(new ParagraphPart(ParagraphPartType.Text, currentTextPart, null));
                        currentTextPart = "";
                    }
                    if (currentCharacter == '`')
                    {
                        var closeIndex = text.IndexOf('`', i + 1);
                        parts.Add(new ParagraphPart(ParagraphPartType.Style, null, "<c>"));
                        if (closeIndex < 0)
                        {
                            closeIndex = text.Length;
                        }
                        parts.Add(new ParagraphPart(ParagraphPartType.Text, text[(i + 1)..(closeIndex - 1)], null));
                        parts.Add(new ParagraphPart(ParagraphPartType.Style, null, "</c>"));
                        i = closeIndex + 1;
                    } 
                    else if (currentCharacter == '*' || currentCharacter == '_')
                    { 
                        if (nextCharacter == '*' || nextCharacter == '_')
                        {
                            if (styles.Contains("bold") && styles.Peek() == "bold")
                            {
                                styles.Pop();
                                parts.Add(new ParagraphPart(ParagraphPartType.Style, null, "</b>"));
                            }
                            else
                            {
                                styles.Push("bold");
                                parts.Add(new ParagraphPart(ParagraphPartType.Style, null, "<b>"));
                            }
                            i += 2;
                        }
                        else
                        {
                            if (styles.Contains("italic") && styles.Peek() == "italic")
                            {
                                styles.Pop();
                                parts.Add(new ParagraphPart(ParagraphPartType.Style, null, "</i>"));
                            }
                            else
                            {
                                styles.Push("italic");
                                parts.Add(new ParagraphPart(ParagraphPartType.Style, null, "<i>"));
                            }
                            i += 1;
                        }
                    }
                }
                if (i < text.Length)
                {
                    currentTextPart += text[i];
                }
            }

            if (!string.IsNullOrEmpty(currentTextPart))
            {
                parts.Add(new ParagraphPart(ParagraphPartType.Text, currentTextPart, null));
            }


            var currentStyle = new List<string>();
            foreach (var part in parts)
            {
                if (part.Type == ParagraphPartType.Style)
                {
                    if (part.Style!.StartsWith("</"))
                    {
                        currentStyle.Remove(part.Style[2].ToString());
                    }
                    else
                    {
                        currentStyle.Add(part.Style[1].ToString());
                    }
                }
                else if (part.Type == ParagraphPartType.Text || part.Type == ParagraphPartType.Link)
                {
                    var run = new Run();
                    if (currentStyle.Any() || part.Type == ParagraphPartType.Link)
                    {
                        var runProperties = new RunProperties();
                        if (currentStyle.Contains("b"))
                        {
                            runProperties.Append(new Bold { Val = OnOffValue.FromBoolean(true) });
                        }
                        if (currentStyle.Contains("i"))
                        {
                            runProperties.Append(new Italic { Val = OnOffValue.FromBoolean(true) });
                        }
                        if (currentStyle.Contains("c"))
                        {
                            runProperties.Append(new RunFonts { Ascii = "Consolas" });
                            runProperties.Append(new Color() { Val = "#915100" });
                        }
                        if (part.Type == ParagraphPartType.Link)
                        {
                            runProperties.Append(new RunStyle { Val = "Hyperlink" });
                        }
                        run.Append(runProperties);
                    }
                    run.Append(new Text(part.Text) { Space = SpaceProcessingModeValues.Preserve });
                    if (part.Type == ParagraphPartType.Text)
                    {
                        paragraph.Append(run);
                    }
                    else
                    {
                        paragraph.Append(new Hyperlink(run) { DocLocation = part.Style });
                    }
                }
            }

            paragraphs.Add(paragraph);
        }
        return paragraphs;
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