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
}
