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
    /// <returns></returns>
    public static FormattedString CreateFormattedString(string text, bool bold = false,
        bool italic = false, Color color = null)
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
}
