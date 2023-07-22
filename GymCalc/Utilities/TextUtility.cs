using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace GymCalc.Utilities;

internal static class TextUtility
{
    /// <summary>
    /// Easily construct a formatted string.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="bold">If it should be bold.</param>
    /// <param name="italic">If it should be italic.</param>
    /// <param name="color">What colour it should be.</param>
    /// <param name="style">What style it should use.</param>
    /// <returns></returns>
    public static FormattedString CreateFormattedString(string text, bool bold = false,
        bool italic = false, Color color = null, Style style = null)
    {
        // Initialize the Span.
        var span = new Span { Text = text };

        // Set the font style if specified.
        if (bold || italic)
        {
            var attr = FontAttributes.None;
            if (bold)
            {
                attr |= FontAttributes.Bold;
            }
            if (italic)
            {
                attr |= FontAttributes.Italic;
            }
            span.FontAttributes = attr;
        }

        // Set the color if specified.
        if (color != null)
        {
            span.TextColor = color;
        }

        // Set the style if specified.
        if (style != null)
        {
            span.Style = style;
        }

        // Construct the FormattedString object.
        return new FormattedString { Spans = { span } };
    }

    public static FormattedString NormalText(string text)
    {
        return CreateFormattedString(text);
    }

    public static FormattedString BoldText(string text)
    {
        return CreateFormattedString(text, true);
    }

    public static FormattedString ItalicText(string text)
    {
        return CreateFormattedString(text, false, true);
    }

    public static FormattedString ColorText(string text, Color color)
    {
        return CreateFormattedString(text, false, false, color);
    }

    public static FormattedString StyleText(string text, Style style)
    {
        return CreateFormattedString(text, false, false, null, style);
    }

    public static FormattedString StyleText(string text, string styleName)
    {
        var style = MauiUtilities.LookupStyle(styleName);
        return CreateFormattedString(text, false, false, null, style);
    }

    public static string CollapseWhitespace(string text)
    {
        return Regex.Replace(text, @"\s{2,}", " ");
    }

    private static void AddSpan(string text, int fontSize, FontAttributes fontAttributes,
        ref List<List<Span>> spans)
    {
        // Ignore empty spans.
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        // Construct a new span.
        var newSpan = new Span
        {
            Text = CollapseWhitespace(text),
            FontSize = fontSize,
            FontAttributes = fontAttributes,
        };

        // Add it to the last span group.
        spans.Last().Add(newSpan);
    }

    public static void ProcessHtmlDocument(HtmlNode node,
        int parentFontSize,
        FontAttributes parentFontAttributes,
        ref List<List<Span>> spans)
    {
        // var node = doc.DocumentNode;
        switch (node.NodeType)
        {
            case HtmlNodeType.Text:
                AddSpan(node.InnerText, parentFontSize, parentFontAttributes, ref spans);
                break;

            case HtmlNodeType.Element:
                // Block-level elements require a new span group (which becomes a Label).
                if (node.Name is "p" or "h1" or "h2" or "li")
                {
                    spans.Add(new List<Span>());
                }

                // Determine the font attributes.
                var fontAttributes = parentFontAttributes;
                switch (node.Name)
                {
                    case "b" or "strong":
                        fontAttributes |= FontAttributes.Bold;
                        break;

                    case "i" or "em":
                        fontAttributes |= FontAttributes.Italic;
                        break;
                }

                // Determine the font size.
                var fontSize = node.Name switch
                {
                    "h1" => 24,
                    "h2" => 20,
                    _ => parentFontSize,
                };

                // Handle unordered lists.
                if (node.Name == "li" && node.ParentNode.Name == "ul")
                {
                    // Add a new span for the bullet.
                    var bullSpan = new Span { Text = "\u2022 " };
                    spans.Last().Add(bullSpan);
                }

                if (node.HasChildNodes)
                {
                    // Add new spans for each child node.
                    foreach (var childNode in node.ChildNodes)
                    {
                        ProcessHtmlDocument(childNode, fontSize, fontAttributes, ref spans);
                    }
                }
                else
                {
                    AddSpan(node.InnerText, fontSize, fontAttributes, ref spans);
                }
                break;

            case HtmlNodeType.Document:
                // Add new spans for each child node.
                foreach (var childNode in node.ChildNodes)
                {
                    ProcessHtmlDocument(childNode, parentFontSize, parentFontAttributes, ref spans);
                }
                break;

            case HtmlNodeType.Comment:
            default:
                throw new ArgumentOutOfRangeException(nameof(node.NodeType), "Unknown node type.");
        }
    }
}
